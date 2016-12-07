using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps.Predefined
{
  public abstract class StepProxyBase<T> : IStep<T> where T : class
  {
    private readonly AsyncStepBase<T> asyncStep;
    private readonly SyncStepBase<T> step;

    public StepProxyBase(AsyncStepBase<T> asyncStep)
    {
      this.asyncStep = asyncStep;
    }

    public StepProxyBase(SyncStepBase<T> step)
    {
      this.step = step;
    }

    public virtual bool CanProcess()
    {
      return this.InnerStep.CanProcess();
    }

    public virtual void Process()
    {
      this.SyncStep.Process();
    }
    
    public virtual async Task ProcessAsync(CancellationToken token)
    {      
      await this.AsyncStep.ProcessAsync(token);      
    }

    private IStep<T> InnerStep
    {
      get { return this.AsyncStep ?? this.SyncStep; }
    }

    private IStep<T> AsyncStep
    {
      get { return this.asyncStep as IStep<T>; }
    }

    private IStep<T> SyncStep
    {
      get { return this.step as IStep<T>; }
    }

    public bool IsAsync()
    {
      return this.asyncStep != null;
    }

    public T Context
    {
      get { return this.InnerStep.Context; }
      set { this.InnerStep.Context = value; }
    }

    public IServiceProvider Services 
    { 
      get { return this.InnerStep.Services; }
      set { this.InnerStep.Services = value; }
    }

    public string Name
    {
      get { return this.InnerStep.Name; }
      set { this.InnerStep.Name = value; }    
    }

    public double TimeTaken 
    {
      get { return this.InnerStep.TimeTaken; }
      set { this.InnerStep.TimeTaken = value; }    
    }

    public int ProcessedItemsCount
    {
      get { return this.InnerStep.ProcessedItemsCount; }
      set { this.InnerStep.ProcessedItemsCount = value; }    
    }
        
    public LinkedList<IStep> Neighbourood
    {
      get { return this.InnerStep.Neighbourood; }
      set { this.InnerStep.Neighbourood = value; }    
    }
    
    public LinkedListNode<IStep> Current
    {
      get { return this.InnerStep.Current; }
      set { this.InnerStep.Current = value; }    
    }
    
    public bool WasFired
    {
      get { return this.InnerStep.WasFired; }
      set { this.InnerStep.WasFired = value; }    
    }
    
    public bool BreakProcessing
    {
      get { return this.InnerStep.BreakProcessing; }
      set { this.InnerStep.BreakProcessing = value; }    
    }
    
  }
}