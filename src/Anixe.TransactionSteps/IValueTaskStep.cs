using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps
{
  public interface IValueTaskStep<T> : IValueTaskStep
  {
    T Context { get; set; }
  }

  public interface IValueTaskStep
  {
    bool CanProcess();
    ValueTask Process(CancellationToken token);
    string Name { get; set; }
    double TimeTaken { get; set; }
    int ProcessedItemsCount { get; set; }
    bool WasFired { get; set; }
    bool BreakProcessing { get; set; }
    /// <summary>
    /// Set up flag if iterator must process step after cancellation occurs via CancellationToken. Flag is valid only for sync steps
    /// </summary>
    /// <returns></returns>
    bool MustProcessAfterCancel { get; set; }
  }
}