﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Anixe.TransactionSteps.Helpers;

namespace Anixe.TransactionSteps
{
  public class StepIterator<T> : IStepIterator<T>
    where T : IPropertyBag
  {
    private readonly T context;
    private readonly List<StepStat> stats;

    public StepIterator(T context)
    {
      this.context = context ?? throw new ArgumentNullException(nameof(context));
      this.stats = new List<StepStat>();
    }

    public List<StepStat> Stats => this.stats;

    public async Task<T> IterateAllAsync(
      IServiceProvider? services,
      LinkedList<IStep> steps,
      IStep? errorHandler,
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

        if (errorHandler.IsAsync())
        {
          await ExecuteStepAsync(errorHandler, token).ConfigureAwait(false);
        }
        else
        {
          ExecuteStep(errorHandler);
        }
      }

      return this.context;
    }

    public void IterateAll(
      IServiceProvider? services,
      LinkedList<IStep> steps,
      IStep? errorHandler)
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

    protected Task<T> Failed(Exception? ex) => Task.FromResult(this.context);

    protected void TakeStats(IStep step) => this.stats.Add(StepStat.CreateFromStep(step));

    private static IStep RetrieveStep(LinkedList<IStep> steps, LinkedListNode<IStep> currentNode, IServiceProvider? services)
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

      var startTimestamp = Stopwatch.GetTimestamp();
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

        var tt = StopwatchHelper.GetElapsedTime(startTimestamp).TotalMilliseconds;
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
        var startTimestamp = Stopwatch.GetTimestamp();
        if (step.IsAsync())
        {
          throw new NotSupportedException("Use IterateAllAsync method");
        }
        else
        {
          step.Process();
        }

        var tt = StopwatchHelper.GetElapsedTime(startTimestamp).TotalMilliseconds;
        step.WasFired = true;
        step.TimeTaken = tt;
        TakeStats(step);
      }
    }
  }
}
