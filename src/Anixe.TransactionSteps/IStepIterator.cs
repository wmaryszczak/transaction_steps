using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps
{
  public interface IStepIterator<T> where T : class
  {
    List<StepStat> Stats { get; }
    Task<T> IterateAllAsync(IServiceProvider services, LinkedList<IStep> steps, CancellationToken token);
  }
}