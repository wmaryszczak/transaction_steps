using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps
{
  public interface IStep
  {
    IServiceProvider Services { get; set; }

    string Name { get; set; }

    double TimeTaken { get; set; }

    int ProcessedItemsCount { get; set; }

    LinkedList<IStep> Neighbourood { get; set; }

    LinkedListNode<IStep> Current { get; set; }

    bool WasFired { get; set; }

    bool BreakProcessing { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether iterator must process step after cancellation occurs via CancellationToken. Flag is valid only for sync steps.
    /// </summary>
    bool MustProcessAfterCancel { get; set; }

    bool CanProcess();

    bool IsAsync();

    Task ProcessAsync(CancellationToken token);

    void Process();
  }
}
