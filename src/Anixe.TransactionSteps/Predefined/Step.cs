using System;
using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps.Predefined
{
  public class Step : StepBase, IStep
  {
    private readonly Action<IStep>? action;
    private readonly Predicate<IStep> canProcessPredicate;

    public Step(Action<IStep>? action, Predicate<IStep>? canProcessPredicate = null)
    {
      this.action = action;
      this.canProcessPredicate = canProcessPredicate ?? True;
    }

    public bool CanProcess() => this.canProcessPredicate(this);

    public bool IsAsync() => false;

    public void Process() => this.action?.Invoke(this);

    public Task ProcessAsync(CancellationToken token) => Task.FromException(new NotImplementedException());

    private bool True(IStep step) => true;
  }
}
