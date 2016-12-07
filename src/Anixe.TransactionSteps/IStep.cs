
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps
{
  public interface IStep<T> : IStep
  {
    T Context { get; set; }
  }

  public interface IStep
  {
    bool CanProcess();
    bool IsAsync();    
    Task ProcessAsync(CancellationToken token);
    void Process();
    IServiceProvider Services { get; set; }
    string Name { get; set; }
    double TimeTaken { get; set; }    
    int ProcessedItemsCount { get; set; }    
    LinkedList<IStep> Neighbourood { get; set; }
    LinkedListNode<IStep> Current { get; set; }
    bool WasFired { get; set; }    
    bool BreakProcessing { get; set; }    
  }
}