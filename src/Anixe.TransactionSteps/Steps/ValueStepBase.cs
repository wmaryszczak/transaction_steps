using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps
{
  public abstract class ValueStepBase : IValueTaskStep
  {
    private string? stepName;

    public string Name
    {
      get
      {
        if (string.IsNullOrEmpty(this.stepName))
        {
          this.stepName = GetDefaultStepName();
        }

        return this.stepName!;
      }
      set => this.stepName = value;
    }

    public double TimeTaken { get; set; }

    public int ProcessedItemsCount { get; set; }

    public bool WasFired { get; set; }

    public bool BreakProcessing { get; set; }

    public bool MustProcessAfterCancel { get; set; } = false;

    public virtual ValueTask Process(CancellationToken token)
    {
      if (IsAsync())
      {
        return new ValueTask(ProcessAsync(token));
      }

      ProcessSync(token);
      return default;
    }

    public abstract bool IsAsync();

    public abstract bool CanProcess();

    protected abstract void ProcessSync(CancellationToken token);

    protected abstract Task ProcessAsync(CancellationToken token);

    private string GetDefaultStepName() => this.GetType().Name;
  }
}
