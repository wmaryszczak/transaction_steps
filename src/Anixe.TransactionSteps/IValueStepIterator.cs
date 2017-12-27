using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps
{
  public interface IValueStepIterator<T> where T : IPropertyBag
  {
    ValueTask<T> IterateAllAsync(
      IServiceProvider services,
      LinkedList<IValueStep<T>> steps,
      IValueStep<T> errorHandler,
      List<StepStatValue> stats,
      CancellationToken token);      
  }
}