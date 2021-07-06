using System;
using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps
{
  public abstract class SyncStepBase<T> : StepBase<T>
    where T : class
  {
    public bool IsAsync() => false;

    public Task ProcessAsync(CancellationToken token) => throw new NotImplementedException();
  }
}
