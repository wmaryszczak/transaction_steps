
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Steps
{
  public interface IStep
  {
    bool CapProcess();
    bool IsAsync();    
    Task ProcessAsync(CancellationToken token);
    void Process();
    IServiceProvider Services { get; set; }
    string Name { get; set; }
    int TimeTaken { get; set; }    
    int ProcessedItemsCount { get; set; }    
    LinkedList<IStep> Neighbourood { get; set; }
    LinkedListNode<IStep> Current { get; set; }
    bool WasFired { get; set; }    
  }
}