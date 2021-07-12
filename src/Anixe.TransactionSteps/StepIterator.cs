﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps
{
  public class StepIterator<T> : IStepIterator<T>
    where T : IPropertyBag
  {
    private readonly T context;
    private readonly List<StepStat> stats;

    public StepIterator(T context)
    {
      this.context = context;
      this.stats = new List<StepStat>();
    }

    public List<StepStat> Stats => this.stats;

    public async Task<T> IterateAllAsync(
      IServiceProvider services,
      LinkedList<IStep> steps,
      IStep errorHandler,
      CancellationToken token)
    {
      var currentNode = steps.First;
      this.stats.Clear();
      try
      {
        while (currentNode != null)
        {
          var step = RetrieveStep(steps, currentNode, services);
          if (!token.IsCancellationRequested || step.MustProcessAfterCancel)
          {
            if (step.IsAsync())
            {
              await ExecuteStepAsync(step, token).ConfigureAwait(false);
            }
            else
            {
              ExecuteStep(step);
            }

            if (step.BreakProcessing)
            {
              break;
            }
          }

          currentNode = currentNode.Next;
        }
      }
      catch (Exception ex) when (errorHandler != null)
      {
        this.context.Set<Exception>(ex);
        errorHandler.Services = services;
        await ExecuteStepAsync(errorHandler, token).ConfigureAwait(false);
      }

      return this.context;
    }

    public void IterateAll(
      IServiceProvider services,
      LinkedList<IStep> steps,
      IStep errorHandler)
    {
      var currentNode = steps.First;
      this.stats.Clear();
      try
      {
        while (currentNode != null)
        {
          var step = RetrieveStep(steps, currentNode, services);
          ExecuteStep(step);
          if (step.BreakProcessing)
          {
            break;
          }

          currentNode = currentNode.Next;
        }
      }
      catch (Exception ex) when (errorHandler != null)
      {
        this.context.Set<Exception>(ex);
        errorHandler.Services = services;
        ExecuteStep(errorHandler);
      }
    }

    protected Task<T> Failed(Exception ex) => Task.FromResult(this.context);

    protected void TakeStats(IStep step) => this.stats.Add(StepStat.CreateFromStep(step));

    private static IStep RetrieveStep(LinkedList<IStep> steps, LinkedListNode<IStep> currentNode, IServiceProvider services)
    {
      var step = currentNode.Value;
      step.Services = services;
      step.Neighbourood = steps;
      step.Current = currentNode;
      return step;
    }

    private async Task ExecuteStepAsync(IStep step, CancellationToken token)
    {
      if (step is IStep<T> stepGeneric)
      {
        stepGeneric.Context = this.context;
      }

      var dt = DateTime.UtcNow;
      if (step.CanProcess())
      {
        if (step.IsAsync())
        {
          await step.ProcessAsync(token).ConfigureAwait(false);
        }
        else
        {
          step.Process();
        }

        var tt = (DateTime.UtcNow - dt).TotalMilliseconds;
        step.WasFired = true;
        step.TimeTaken = tt;
        TakeStats(step);
      }
    }

    private void ExecuteStep(IStep step)
    {
      if (step is IStep<T> stepGeneric)
      {
        stepGeneric.Context = this.context;
      }

      if (step.CanProcess())
      {
        var dt = DateTime.UtcNow;
        if (step.IsAsync())
        {
          throw new NotSupportedException("Use IterateAllAsync method");
        }
        else
        {
          step.Process();
        }

        var tt = (DateTime.UtcNow - dt).TotalMilliseconds;
        step.WasFired = true;
        step.TimeTaken = tt;
        TakeStats(step);
      }
    }
  }
}
