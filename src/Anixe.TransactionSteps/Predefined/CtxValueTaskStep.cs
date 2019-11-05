using System;
using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps.Predefined
{
  public class CtxValueTaskStep<T> : ValueStepBase<T>, IValueTaskStep<T> where T : class
  {
    private readonly Action<IValueTaskStep<T>> syncAction;
    private readonly Func<IValueTaskStep<T>, CancellationToken, Task> asyncAction;
    private readonly Func<bool> canProcessPredicate;

    public CtxValueTaskStep(Action<IValueTaskStep<T>> syncAction, Func<IValueTaskStep<T>, CancellationToken, Task> asyncAction, Func<bool> canProcessPredicate)
    {
      this.syncAction = syncAction;
      this.asyncAction = asyncAction;
      this.canProcessPredicate = canProcessPredicate;
    }

    public override bool CanProcess()
    {
      return this.canProcessPredicate();
    }

    public override bool IsAsync()
    {
      return this.asyncAction != null;
    }

    protected override Task ProceeAsync(CancellationToken token)
    {
      return this.asyncAction(this, token);
    }

    protected override void ProcessSync(CancellationToken token)
    {
      this.syncAction(this);
    }
  }
}