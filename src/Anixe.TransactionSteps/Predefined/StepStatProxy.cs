using System;
using System.Threading;
using System.Threading.Tasks;

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
      var dt = DateTime.UtcNow;
      base.Process();
      var tt = (DateTime.UtcNow - dt).TotalMilliseconds;
      this.TimeTaken = tt;
    }

    public override async Task ProcessAsync(CancellationToken token)
    {
      var dt = DateTime.UtcNow;
      await base.ProcessAsync(token).ConfigureAwait(false);
      var tt = (DateTime.UtcNow - dt).TotalMilliseconds;
      this.TimeTaken = tt;
    }
  }
}
