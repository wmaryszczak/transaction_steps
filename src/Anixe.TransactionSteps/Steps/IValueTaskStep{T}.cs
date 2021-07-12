namespace Anixe.TransactionSteps
{
  public interface IValueTaskStep<T> : IValueTaskStep
  {
    T Context { get; set; }
  }
}
