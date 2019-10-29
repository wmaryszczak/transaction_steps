namespace Anixe.TransactionSteps.Benchmark
{
  internal class SyncStep : SyncStepBase<IPropertyBag>, IStep<IPropertyBag>
  {
    private int tmp;
    public bool CanProcess()
    {
      return true;
    }

    public void Process()
    {
      tmp++;
    }
  }
}
