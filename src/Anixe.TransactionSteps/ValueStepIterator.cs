using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps
{
  public class ValueStepIterator<T> : IValueStepIterator<T> where T : IPropertyBag
  {
    private readonly T context;

    public ValueStepIterator(T context)
    {
      this.context = context;
    }

    public ValueTask<T> IterateAllAsync(
      IServiceProvider services,
      LinkedList<IValueStep<T>> steps,
      IValueStep<T> errorHandler,
      List<StepStatValue> stats,
      CancellationToken token)
    {
      var currentNode = steps.First;
      stats.Clear();
      try
      {
        while (currentNode != null)
        {
          var step = RetrieveStep(steps, currentNode, services);
          if (!token.IsCancellationRequested || step.MustProcessAfterCancel)
          {
            ExecuteStepAsync(step, stats, token);
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
          errorHandler.Context = context;
          ExecuteStepAsync(errorHandler, stats, token);
        }
        else
        {
          throw;
        }
      }
      return new ValueTask<T>(this.context);
    }

    private IValueStep<T> RetrieveStep(LinkedList<IValueStep<T>> steps, LinkedListNode<IValueStep<T>> currentNode, IServiceProvider services)
    {
      var step = currentNode.Value;
      step.Services = services;
      step.Neighbourood = steps;
      step.Current = currentNode;
      return step;
    }

    private ValueTask<T> ExecuteStepAsync(IValueStep<T> step, List<StepStatValue> stats, CancellationToken token)
    {
      step.Context = this.context;
      var dt = DateTime.UtcNow;
      if (step.CanProcess())
      {
        var r = step.ProcessAsync(token).Result;
        var tt = (DateTime.UtcNow - dt).TotalMilliseconds;
        step.WasFired = true;
        step.TimeTaken = tt;
        stats.Add(StepStatValue.CreateFromStep(step));
      }
      return new ValueTask<T>(this.context);
    }
  }
}