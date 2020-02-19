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

    protected StepProxyBase(AsyncStepBase<T> asyncStep)
    {
      this.asyncStep = asyncStep;
    }

    protected StepProxyBase(SyncStepBase<T> step)
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

    public virtual Task ProcessAsync(CancellationToken token)
    {
      return this.AsyncStep.ProcessAsync(token);
    }

    private IStep<T> InnerStep
    {
      get { return this.AsyncStep ?? this.SyncStep; }
    }

    private IStep<T> AsyncStep
    {
      get => this.asyncStep as IStep<T>;
    }

    private IStep<T> SyncStep
    {
      get => this.step as IStep<T>;
    }

    public bool IsAsync()
    {
      return this.asyncStep != null;
    }

    public T Context
    {
      get => this.InnerStep.Context;
      set => this.InnerStep.Context = value;
    }

    public IServiceProvider Services
    {
      get => this.InnerStep.Services;
      set => this.InnerStep.Services = value;
    }

    public string Name
    {
      get => this.InnerStep.Name;
      set => this.InnerStep.Name = value;
    }

    public double TimeTaken
    {
      get => this.InnerStep.TimeTaken;
      set => this.InnerStep.TimeTaken = value;
    }

    public int ProcessedItemsCount
    {
      get => this.InnerStep.ProcessedItemsCount;
      set => this.InnerStep.ProcessedItemsCount = value;
    }

    public LinkedList<IStep> Neighbourood
    {
      get => this.InnerStep.Neighbourood;
      set => this.InnerStep.Neighbourood = value;
    }

    public LinkedListNode<IStep> Current
    {
      get => this.InnerStep.Current;
      set => this.InnerStep.Current = value;
    }

    public bool WasFired
    {
      get => this.InnerStep.WasFired;
      set => this.InnerStep.WasFired = value;
    }

    public bool BreakProcessing
    {
      get => this.InnerStep.BreakProcessing;
      set => this.InnerStep.BreakProcessing = value;
    }

    public bool MustProcessAfterCancel
    {
      get => this.InnerStep.MustProcessAfterCancel;
      set => this.InnerStep.MustProcessAfterCancel = value;
    }
  }
}