using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps
{
  public abstract class AsyncValueStepBase<T> : StepBase where T : class
  {
    public bool IsAsync()
    {
      return true;
    }
    public void Process()
    {
      throw new NotImplementedException();
    }

    public T Context
    {
      get;
      set;
    }

    public LinkedList<IValueStep<T>> Neighbourood { get; set; }
    public LinkedListNode<IValueStep<T>> Current { get; set; }
  }

  public abstract class AsyncStepBase<T> : StepBase<T> where T : class
  {
    public bool IsAsync()
    {
      return true;
    }
    public void Process()
    {
      throw new NotImplementedException();
    }
  }

  public abstract class SyncStepBase<T> : StepBase<T> where T : class
  {
    public bool IsAsync()
    {
      return false;
    }
    public Task<T> ProcessAsync(CancellationToken token)
    {
      throw new NotImplementedException();
    }
  }
  
  public abstract class StepBase<T> : StepBase where T : class
  {
    public T Context
    {
      get;
      set;
    }

    public LinkedList<IStep<T>> Neighbourood { get; set; }
    public LinkedListNode<IStep<T>> Current { get; set; }
  }

  public abstract class StepBase
  {
    private string stepName;
    public IServiceProvider Services { get; set; }
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