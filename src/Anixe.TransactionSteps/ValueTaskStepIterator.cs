using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps
{
  public class ValueTaskStepIterator<T> : IValueTaskStepIterator<T>
  {
    public async ValueTask<List<StepStat>> IterateAllAsync(
      T context,
      IList<IValueTaskStep> steps,
      CancellationToken token)
    {
      var stats = new List<StepStat>(1);//steps.Count);
      for (int i = 0; i < steps.Count; i++)
      {
        var step = steps[i];
        if (!token.IsCancellationRequested || step.MustProcessAfterCancel)
        {
          var t = ExecuteStep(step, context, stats, token);
          if (!t.IsCompletedSuccessfully)
          {
            await t;
          }
          if (step.BreakProcessing)
          {
            break;
          }
        }
      }
      return stats;
    }

    private async ValueTask ExecuteStep(IValueTaskStep step, T context, List<StepStat> stats, CancellationToken token)
    {
      if (step is IValueTaskStep<T> stepWithCtx)
      {
        stepWithCtx.Context = context;
      }

      if (step.CanProcess())
      {
        var dt = DateTime.UtcNow;
        await step.Process(token);
        var tt = (DateTime.UtcNow - dt).TotalMilliseconds;
        step.WasFired = true;
        step.TimeTaken = tt;
        stats.Add(StepStat.CreateFromStep(step));
      }
    }
  }
}