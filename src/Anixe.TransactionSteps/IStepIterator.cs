using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps
{
  public interface IStepIterator<T> where T : IPropertyBag
  {
    List<StepStat> Stats { get; }
    Task<T> IterateAllAsync(
      IServiceProvider services, 
      LinkedList<IStep<T>> steps, 
      IStep<T> errorHandler, 
      CancellationToken token);

    void IterateAll(
      IServiceProvider services, 
      LinkedList<IStep<T>> steps, 
      IStep errorHandler);
      
  }
}