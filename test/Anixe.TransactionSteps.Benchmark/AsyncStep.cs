using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps.Benchmark
{
  internal class AsyncStep : AsyncStepBase<IPropertyBag>, IStep<IPropertyBag>
  {
    public bool CanProcess()
    {
      return true;
    }

    public Task ProcessAsync(CancellationToken token)
    {
      return Task.FromResult(true);
    }
  }
}