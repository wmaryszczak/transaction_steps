namespace Anixe.TransactionSteps
{
  public abstract class StepBase<T> : StepBase
    where T : class
  {
    public T Context { get; set; } = null!;
  }
}
