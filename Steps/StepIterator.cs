
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Steps
{
  public class StepIterator
  {
    public async Task IterateAllAsync(IServiceProvider services, List<IStep> steps)
    {
      for (int i = 0; i < steps.Count; i++)
      {
        var step = steps[i];
        if(step.CapProcess())
        {
          var dt = DateTime.UtcNow;
          step.Services = services;
          if(step.IsAsync())
          {
            await step.ProcessAsync();
          }
          else 
          {
            step.Process();
          }
          var tt = (DateTime.UtcNow - dt).TotalMilliseconds;
        }
      }
    }
  }
}