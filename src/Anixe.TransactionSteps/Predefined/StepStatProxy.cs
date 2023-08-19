using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Anixe.TransactionSteps.Helpers;

namespace Anixe.TransactionSteps.Predefined
{
  public abstract class StepStatProxy<T> : StepProxyBase<T>
    where T : class
  {
    protected StepStatProxy(AsyncStepBase<T> asyncStep)
      : base(asyncStep)
    {
    }

    protected StepStatProxy(SyncStepBase<T> step)
      : base(step)
    {
    }

    public override void Process()
    {
      var startTimestamp = Stopwatch.GetTimestamp();
      base.Process();
      var tt = StopwatchHelper.GetElapsedTime(startTimestamp).TotalMilliseconds;
      this.TimeTaken = tt;
    }

    public override async Task ProcessAsync(CancellationToken token)
    {
      var startTimestamp = Stopwatch.GetTimestamp();
      await base.ProcessAsync(token).ConfigureAwait(false);
      var tt = StopwatchHelper.GetElapsedTime(startTimestamp).TotalMilliseconds;
      this.TimeTaken = tt;
    }
  }
}
