using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps
{
  public interface IValueTaskStepIterator<T>
  {
    ValueTask<List<StepStat>> IterateAllAsync(
      T context,
      IList<IValueTaskStep> steps,
      CancellationToken token);
  }
}
