namespace Anixe.TransactionSteps
{
  public interface IStep<T> : IStep
  {
    T Context { get; set; }
  }
}
