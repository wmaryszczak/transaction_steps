using System;
using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps.Predefined
{
  public class CtxStep<T> : StepBase<T>, IStep<T> where T : class
  {
    private Action<IStep<T>> action;
    private Predicate<IStep<T>> canProcessPredicate;

    public CtxStep(Action<IStep<T>> action, Predicate<IStep<T>> canProcessPredicate = null)
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
        return false;
    }

    public void Process()
    {
      if(this.action != null)
      {
        this.action(this);
      }
    }

    public async Task ProcessAsync(CancellationToken token)
    {
      #if !NET45
      await Task.FromException(new NotImplementedException());
      #else
      var ctx = new TaskCompletionSource<bool>();
      ctx.SetException(new NotImplementedException());
      await ctx.Task;
      #endif
    }

    private bool True(IStep step)
    {
      return true;
    }    
  }
}