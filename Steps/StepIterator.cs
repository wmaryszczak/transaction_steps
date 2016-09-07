
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Steps
{
  public class StepIterator
  {
    public async Task<int> IterateAllAsync(IServiceProvider services, LinkedList<IStep> steps, CancellationToken token)
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
          if(step.CanProcess())
          {
            step.Services = services;
            step.Neighbourood = steps;
            step.Current = currentNode;
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
      return stepsExecuted;
    }

    private async Task<int> Failed(Exception ex)
    {
      var tcs = new TaskCompletionSource<int>();
      tcs.SetException(ex);
      return await tcs.Task;        
    }

    private async Task<int> Cancelled()
    {
      var tcs = new TaskCompletionSource<int>();
      tcs.SetCanceled();
      return await tcs.Task;        
    }
  }
}