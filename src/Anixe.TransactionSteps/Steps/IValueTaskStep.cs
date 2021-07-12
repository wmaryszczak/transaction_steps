using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps
{
  public interface IValueTaskStep
  {
    string Name { get; set; }

    double TimeTaken { get; set; }

    int ProcessedItemsCount { get; set; }

    bool WasFired { get; set; }

    bool BreakProcessing { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether iterator must process step after cancellation occurs via CancellationToken. Flag is valid only for sync steps.
    /// </summary>
    bool MustProcessAfterCancel { get; set; }

    bool CanProcess();

    ValueTask Process(CancellationToken token);
  }
}
