
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps
{
  public class StepIterator<T> where T : class
  {
    private readonly T context;
    public StepIterator(T context)
    {
      this.context = context;
    }

    public async Task<T> IterateAllAsync(IServiceProvider services, LinkedList<IStep> steps, CancellationToken token)
    {
      var currentNode = steps.First;
      int stepsExecuted = 0;
      try
      {
        while(currentNode != null)
        {
          if(token.IsCancellationRequested)
          {
            return await Cancelled();
          }
          var step = currentNode.Value;
          step.Services = services;
          step.Neighbourood = steps;
          step.Current = currentNode;
          if(step is IStep<T>)
          {
            ((IStep<T>)step).Context = this.context;
          }
          if(step.CanProcess())
          {
            var dt = DateTime.UtcNow;
            if(step.IsAsync())
            {
              await step.ProcessAsync(token);
            }
            else 
            {
              step.Process();
            }
            var tt = (DateTime.UtcNow - dt).TotalMilliseconds;
            step.WasFired = true;
            step.TimeTaken = (int)tt;
            stepsExecuted++;
            if(step.BreakProcessing)
            {
              break;
            }
          }
          currentNode = currentNode.Next;
        }
      }
      catch(Exception ex)
      {
        return await Failed(ex);
      }
      return this.context;      
    }

    protected async Task<T> Failed(Exception ex)
    {
      var tcs = new TaskCompletionSource<T>();
      tcs.SetException(ex);
      return await tcs.Task;        
    }

    protected async Task<T> Cancelled()
    {
      var tcs = new TaskCompletionSource<T>();
      tcs.SetCanceled();
      return await tcs.Task;        
    }
    
  }

  public class StepIterator : StepIterator<object>
  {
    public StepIterator()
      :base(new object())
    {
    }
  }
}