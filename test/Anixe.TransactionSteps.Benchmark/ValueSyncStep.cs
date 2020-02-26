using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps.Benchmark
{
  public class ValueSyncStep : ValueStepBase<int>, IValueTaskStep<int>
  {
    public override bool CanProcess()
    {
      return true;
    }

    public override bool IsAsync()
    {
      return false;
    }

    protected override Task ProcessAsync(CancellationToken token)
    {
      throw new System.NotImplementedException();
    }

    protected override void ProcessSync(CancellationToken token)
    {
      this.Context++;
    }
  }
}