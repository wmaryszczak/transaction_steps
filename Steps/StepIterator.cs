
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Steps
{
  public class StepIterator
  {
    public async Task IterateAllAsync(IServiceProvider services, LinkedList<IStep> steps, CancellationToken token)
    {
      var currentNode = steps.First;
      while(currentNode != null)
      {
        var step = currentNode.Value;
        if(step.CapProcess())
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
        }
        currentNode = currentNode.Next;
      }
    }
  }
}