using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps.Predefined
{
  public abstract class StepProxyBase<T> : IStep<T>
    where T : class
  {
    private readonly AsyncStepBase<T>? asyncStep;
    private readonly SyncStepBase<T>? step;

    protected StepProxyBase(AsyncStepBase<T> asyncStep)
    {
      this.asyncStep = asyncStep ?? throw new ArgumentNullException(nameof(asyncStep));
    }

    protected StepProxyBase(SyncStepBase<T> step)
    {
      this.step = step ?? throw new ArgumentNullException(nameof(step));
    }

    public T Context
    {
      get => this.InnerStep.Context;
      set => this.InnerStep.Context = value;
    }

    public IServiceProvider? Services
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

    private IStep<T> InnerStep => this.AsyncStep ?? this.SyncStep ?? throw new InvalidOperationException("No step defined");

    private IStep<T>? AsyncStep => this.asyncStep as IStep<T>;

    private IStep<T>? SyncStep => this.step as IStep<T>;

    public virtual bool CanProcess() => this.InnerStep.CanProcess();

    public virtual void Process()
    {
      if (this.SyncStep is null)
      {
        throw new NotSupportedException("Synchronous processing of the step is not supported because is asynchronous. Use ProcessAsync instead.");
      }

      this.SyncStep.Process();
    }

    public virtual Task ProcessAsync(CancellationToken token)
    {
      if (this.AsyncStep is null)
      {
        return Task.FromException(new NotSupportedException("Asynchronous processing of the step is not supported because is synchronous. Use Process instead."));
      }

      return this.AsyncStep.ProcessAsync(token);
    }

    public bool IsAsync() => this.asyncStep != null;
  }
}
