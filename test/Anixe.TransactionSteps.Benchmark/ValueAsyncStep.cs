using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps.Benchmark
{
  public class ValueAsyncStep : ValueStepBase<int>, IValueTaskStep<int>
  {
    public override bool CanProcess()
    {
      return true;
    }

    public override bool IsAsync()
    {
      return true;
    }

    protected override Task ProcessAsync(CancellationToken token)
    {
      this.Context++;
      return Task.FromResult(true);
    }

    protected override void ProcessSync(CancellationToken token)
    {
      throw new System.NotImplementedException();
    }
  }
}