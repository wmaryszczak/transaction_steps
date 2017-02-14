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
      get{ return this.stats; }
    }

    public StepIterator(T context)
    {
      this.context = context;
      this.stats = new List<StepStat>{ };
    }

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
        while(currentNode != null)
        {
          var step = currentNode.Value;
          step.Services = services;
          step.Neighbourood = steps;
          step.Current = currentNode;
          if(!token.IsCancellationRequested || step.MustProcessAfterCancel)
          {
            await ExecuteStep(step, token);
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
        if(errorHandler != null)
        {
          this.context.Set<Exception>(ex);
          errorHandler.Services = services;
          await ExecuteStep(errorHandler, token);
        }
        else
        {
          throw;
        }
      }

      return this.context;      

    }

    private async Task ExecuteStep(IStep step, CancellationToken token)
    {
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