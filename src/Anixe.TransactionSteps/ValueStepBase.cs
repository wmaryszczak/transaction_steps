using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace Anixe.TransactionSteps
{
  
  public abstract class ValueStepBase<T> : ValueStepBase
  {
    public T Context
    {
      get;
      set;
    }
  }

  public abstract class ValueStepBase : IValueTaskStep
  {
    public virtual ValueTask Process(CancellationToken token)
    {
      if(IsAsync())
      {
        return new ValueTask(ProceeAsync(token));
      }
      ProcessSync(token);
      return new ValueTask();
    }

    protected abstract void ProcessSync(CancellationToken token);

    protected abstract Task ProceeAsync(CancellationToken token);

    public abstract bool IsAsync();
    public abstract bool CanProcess();

    private string stepName;
    public string Name 
    { 
      get 
      {
        if(string.IsNullOrEmpty(this.stepName))
        {
          this.stepName = GetDefaultStepName();
        }
        return this.stepName;
      } 
      set
      {
        this.stepName = value;
      } 
    }
    public double TimeTaken { get; set; }
    public int ProcessedItemsCount { get; set; }    
    public bool WasFired { get; set; }
    public bool BreakProcessing { get; set; }
    public bool MustProcessAfterCancel { get; set; } = false;

    private string GetDefaultStepName()
    {
      return this.GetType().Name;
    }
  }
}