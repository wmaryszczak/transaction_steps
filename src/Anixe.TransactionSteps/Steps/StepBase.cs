using System;
using System.Collections.Generic;

namespace Anixe.TransactionSteps
{
  public abstract class StepBase
  {
    private string stepName;

    public IServiceProvider Services { get; set; }

    public string Name
    {
      get
      {
        if (string.IsNullOrEmpty(this.stepName))
        {
          this.stepName = GetDefaultStepName();
        }

        return this.stepName;
      }
      set => this.stepName = value;
    }

    public double TimeTaken { get; set; }

    public int ProcessedItemsCount { get; set; }

    public LinkedList<IStep> Neighbourood { get; set; }

    public LinkedListNode<IStep> Current { get; set; }

    public bool WasFired { get; set; }

    public bool BreakProcessing { get; set; }

    public bool MustProcessAfterCancel { get; set; } = false;

    private string GetDefaultStepName() => this.GetType().Name;
  }
}
