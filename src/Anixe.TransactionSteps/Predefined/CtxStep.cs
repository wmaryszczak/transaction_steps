using System;
using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps.Predefined
{
  public class CtxStep<T> : StepBase<T>, IStep<T>
    where T : class
  {
    private readonly Action<IStep<T>> action;
    private readonly Predicate<IStep<T>> canProcessPredicate;

    public CtxStep(Action<IStep<T>> action, Predicate<IStep<T>>? canProcessPredicate = null)
    {
      this.action = action ?? throw new ArgumentNullException(nameof(action));
      this.canProcessPredicate = canProcessPredicate ?? True;
    }

    public bool CanProcess() => this.canProcessPredicate(this);

    public bool IsAsync() => false;

    public void Process() => this.action?.Invoke(this);

    public Task ProcessAsync(CancellationToken token) => Task.FromException(new NotImplementedException());

    private bool True(IStep step) => true;
  }
}
