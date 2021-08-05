using System;
using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps.Predefined
{
  public class AsyncStep : StepBase, IStep
  {
    private readonly Func<IStep, Task> action;
    private readonly Predicate<IStep> canProcessPredicate;

    public AsyncStep(Func<IStep, Task> action, Predicate<IStep>? canProcessPredicate = null)
    {
      this.action = action ?? throw new ArgumentNullException(nameof(action));
      this.canProcessPredicate = canProcessPredicate ?? this.True;
    }

    public bool CanProcess() => this.canProcessPredicate(this);

    public bool IsAsync() => true;

    public void Process() => throw new NotSupportedException();

    public Task ProcessAsync(CancellationToken token)
    {
      if (token.IsCancellationRequested)
      {
        var tcs = new TaskCompletionSource<int>();
        tcs.SetCanceled();
        return tcs.Task;
      }

      return this.action(this);
    }

    private bool True(IStep step) => true;
  }
}
