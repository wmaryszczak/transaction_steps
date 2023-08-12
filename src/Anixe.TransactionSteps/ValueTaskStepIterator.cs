using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Anixe.TransactionSteps.Helpers;

namespace Anixe.TransactionSteps
{
  public class ValueTaskStepIterator<T> : IValueTaskStepIterator<T>
  {
    public async ValueTask<List<StepStat>> IterateAllAsync(
      T context,
      IList<IValueTaskStep> steps,
      CancellationToken token)
    {
      if (context is null)
      {
        throw new ArgumentNullException(nameof(context));
      }

      if (steps is null)
      {
        throw new ArgumentNullException(nameof(steps));
      }

      var stats = new List<StepStat>(steps.Count);
      for (int i = 0; i < steps.Count; i++)
      {
        var step = steps[i];
        if (!token.IsCancellationRequested || step.MustProcessAfterCancel)
        {
          var t = ExecuteStep(step, context, stats, token);
          if (!t.IsCompletedSuccessfully)
          {
            await t.ConfigureAwait(false);
          }

          if (step.BreakProcessing)
          {
            break;
          }
        }
      }

      return stats;
    }

    private static async ValueTask ExecuteStep(IValueTaskStep step, T context, List<StepStat> stats, CancellationToken token)
    {
      if (step is IValueTaskStep<T> stepWithCtx)
      {
        stepWithCtx.Context = context;
      }

      if (step.CanProcess())
      {
        var startTimestamp = Stopwatch.GetTimestamp();
        await step.Process(token).ConfigureAwait(false);
        var tt = StopwatchHelper.GetElapsedTime(startTimestamp).TotalMilliseconds;
        step.WasFired = true;
        step.TimeTaken = tt;
        stats.Add(StepStat.CreateFromStep(step));
      }
    }
  }
}
