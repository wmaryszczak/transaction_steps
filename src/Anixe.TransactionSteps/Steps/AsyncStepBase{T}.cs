using System;

namespace Anixe.TransactionSteps
{
  public abstract class AsyncStepBase<T> : StepBase<T>
    where T : class
  {
    public bool IsAsync() => true;

    public void Process() => throw new NotImplementedException();
  }
}
