using System;
using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps.Predefined
{
  public class CtxValueTaskStep<T> : ValueStepBase<T>, IValueTaskStep<T>
    where T : class
  {
    private readonly Action<IValueTaskStep<T>>? syncAction;
    private readonly Func<IValueTaskStep<T>, CancellationToken, Task>? asyncAction;
    private readonly Func<bool> canProcessPredicate;

    public CtxValueTaskStep(Action<IValueTaskStep<T>>? syncAction, Func<IValueTaskStep<T>, CancellationToken, Task>? asyncAction, Func<bool> canProcessPredicate)
    {
      if (asyncAction is null && syncAction is null)
      {
        throw new ArgumentException($"Either {asyncAction} or {syncAction} must be not null");
      }

      this.syncAction = syncAction;
      this.asyncAction = asyncAction;
      this.canProcessPredicate = canProcessPredicate ?? throw new ArgumentNullException(nameof(canProcessPredicate));
    }

    public override bool CanProcess() => this.canProcessPredicate();

    public override bool IsAsync() => this.asyncAction != null;

    protected override Task ProcessAsync(CancellationToken token) => this.asyncAction!(this, token);

    protected override void ProcessSync(CancellationToken token) => this.syncAction!(this);
  }
}
