using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps
{
  public class StepIterator<T> : IStepIterator<T> where T : IPropertyBag
  {
    private readonly T context;
    private readonly List<StepStat> stats;

    public List<StepStat> Stats
    {
      get { return this.stats; }
    }

    public StepIterator(T context)
    {
      this.context = context;
      this.stats = new List<StepStat> { };
    }

    public async Task<T> IterateAllAsync(
      IServiceProvider services,
      LinkedList<IStep<T>> steps,
      IStep<T> errorHandler,
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
            await ExecuteStepAsync(step, token);
            if (step.BreakProcessing)
            {
              break;
            }
          }
          currentNode = currentNode.Next;
        }
      }
      catch (Exception ex)
      {
        if (errorHandler != null)
        {
          this.context.Set<Exception>(ex);
          errorHandler.Services = services;
          await ExecuteStepAsync(errorHandler, token);
        }
        else
        {
          throw;
        }
      }
      return this.context;
    }


    public ValueTask<T> IterateAllAsync(
      IServiceProvider services,
      LinkedList<IValueStep<T>> steps,
      IValueStep<T> errorHandler,
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
            ExecuteStepAsync(step, token);
            if (step.BreakProcessing)
            {
              break;
            }
          }
          currentNode = currentNode.Next;
        }
      }
      catch (Exception ex)
      {
        if (errorHandler != null)
        {
          this.context.Set<Exception>(ex);
          errorHandler.Services = services;
          ExecuteStepAsync(errorHandler, token);
        }
        else
        {
          throw;
        }
      }
      return new ValueTask<T>(this.context);
    }

    public void IterateAll(
      IServiceProvider services,
      LinkedList<IStep<T>> steps,
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
      catch (Exception ex)
      {
        if (errorHandler != null)
        {
          this.context.Set<Exception>(ex);
          errorHandler.Services = services;
          ExecuteStep(errorHandler);
        }
        else
        {
          throw;
        }
      }
    }

    private IStep<T> RetrieveStep(LinkedList<IStep<T>> steps, LinkedListNode<IStep<T>> currentNode, IServiceProvider services)
    {
      var step = currentNode.Value;
      step.Services = services;
      step.Neighbourood = steps;
      step.Current = currentNode;
      return step;
    }

    private IValueStep<T> RetrieveStep(LinkedList<IValueStep<T>> steps, LinkedListNode<IValueStep<T>> currentNode, IServiceProvider services)
    {
      var step = currentNode.Value;
      step.Services = services;
      step.Neighbourood = steps;
      step.Current = currentNode;
      return step;
    }

    private async Task ExecuteStepAsync(IStep<T> step, CancellationToken token)
    {
      if (step is IStep<T>)
      {
        ((IStep<T>)step).Context = this.context;
      }

      var dt = DateTime.UtcNow;
      if (step.CanProcess())
      {
        if (step.IsAsync())
        {
          await step.ProcessAsync(token);
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

    private ValueTask<T> ExecuteStepAsync(IValueStep<T> step, CancellationToken token)
    {
      step.Context = this.context;
      var dt = DateTime.UtcNow;
      if (step.CanProcess())
      {
        var r = step.ProcessAsync(token).Result;
        var tt = (DateTime.UtcNow - dt).TotalMilliseconds;
        step.WasFired = true;
        step.TimeTaken = tt;
        TakeStats(step);
      }
      return new ValueTask<T>(this.context);
    }

    private void ExecuteStep(IStep step)
    {
      if (step is IStep<T>)
      {
        ((IStep<T>)step).Context = this.context;
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

    protected async Task<T> Failed(Exception ex)
    {
      var tcs = new TaskCompletionSource<T>();
      tcs.SetResult(this.context);
      return await tcs.Task;
    }

    protected void TakeStats(IStep step)
    {
      this.stats.Add(StepStat.CreateFromStep(step));
    }
  }
}