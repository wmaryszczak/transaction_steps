using System;
using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps.Predefined
{
  public class AsyncStep : StepBase, IStep
  {
    private Func<IStep, Task> action;
    private Predicate<IStep> canProcessPredicate;
    

    public AsyncStep(Func<IStep, Task> action, Predicate<IStep> canProcessPredicate = null)
    {
      this.action = action;        
      this.canProcessPredicate = canProcessPredicate ?? True;      
    }
    
    public bool CanProcess()
    {
        return this.canProcessPredicate(this);
    }

    public bool IsAsync()
    {
        return true;
    }

    public void Process()
    {
      throw new NotImplementedException();
    }

    public async Task ProcessAsync(CancellationToken token)
    {
      if(token.IsCancellationRequested)
      {
        var tcs = new TaskCompletionSource<int>();
        tcs.SetCanceled();
        await tcs.Task;        
      }
      await this.action(this);
    }

    private bool True(IStep step)
    {
      return true;
    }        
  }
}