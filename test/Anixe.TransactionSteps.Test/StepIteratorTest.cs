using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anixe.TransactionSteps.Predefined;
using Xunit;

namespace Anixe.TransactionSteps.Test
{
  public class StepIteratorTest
  {
    private PropertyBag ctx;
    private LinkedList<IStep> steps;
    private StepIterator<IPropertyBag> subject;

    public StepIteratorTest()
    {
      this.ctx = new PropertyBag{};
      this.steps = CreateSteps();
      this.subject = new StepIterator<IPropertyBag>(ctx);      
    }


    public LinkedList<IStep> CreateSteps()
    {
      return new LinkedList<IStep>
      (
        new List<IStep>
        { 
          new CtxStep<IPropertyBag>((_) => {  }, (_) => { return true; }),
          new CtxStep<IPropertyBag>((_) => {  }, (_) => { return true; }),
        }
      );
    }
      
    [Fact]
    public async Task Should_Iterate_Over_All_Steps()
    {
      await this.subject.IterateAllAsync(null, steps, null, new CancellationToken());
      Assert.True(steps.All(s => s.WasFired));
    }

    [Fact]
    public async Task Should_Iterate_Over_Steps_Which_Can_Process()
    {
      steps.AddFirst(new CtxStep<IPropertyBag>((_) => {  }, (_) => { return false; }));
      await this.subject.IterateAllAsync(null, steps, null, new CancellationToken());
      Assert.False(steps.All(s => s.WasFired));
      Assert.Equal(2, steps.Where(s => s.WasFired).Count());
    }

    [Fact]
    public async Task Should_Not_Iterate_Over_Steps_After_Cancel()
    {
      using(var cts = new CancellationTokenSource())
      {
        steps.AddLast(new CtxStep<IPropertyBag>((_) => { cts.Cancel(); }, (_) => { return true; }));
        steps.AddLast(new CtxStep<IPropertyBag>((_) => {   }, (_) => { return true; }));
        await this.subject.IterateAllAsync(null, steps, null, cts.Token);
        Assert.False(steps.All(s => s.WasFired));
        Assert.Equal(3, steps.Where(s => s.WasFired).Count());
      }
    }
    
    [Fact]
    public async Task Should_Iterate_Over_MustProcessAfterCancel_Steps_After_Cancel()
    {
      using(var cts = new CancellationTokenSource())
      {
        steps.AddLast(new CtxStep<IPropertyBag>((_) => { cts.Cancel(); }, (_) => { return true; }));
        steps.AddLast(new CtxStep<IPropertyBag>((_) => {   }, (_) => { return true; }){ MustProcessAfterCancel = true });
        await this.subject.IterateAllAsync(null, steps, null, cts.Token);
        Assert.True(steps.All(s => s.WasFired));
      }
    }

    [Fact]
    public async Task Should_Fire_Error_Handler_On_Excepton()
    {
      var errorHandler = new CtxStep<IPropertyBag>((s) => { Assert.True(s.Context.Contains<Exception>()); }, (_) => { return true; });
      steps.AddFirst(new CtxStep<IPropertyBag>((_) => {  throw new InvalidOperationException("test"); }, (_) => { return true; }));
      await this.subject.IterateAllAsync(null, steps, errorHandler, new CancellationToken());
      Assert.True(errorHandler.WasFired);
    }    
  }
}