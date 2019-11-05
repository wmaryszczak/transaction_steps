using System;
using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps.Predefined
{
  public class CtxStep<T> : StepBase<T>, IStep<T> where T : class
  {
    private readonly Action<IStep<T>> action;
    private readonly Predicate<IStep<T>> canProcessPredicate;

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
      this.action?.Invoke(this);
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