using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anixe.TransactionSteps.Predefined;
using Xunit;

namespace Anixe.TransactionSteps.Test
{
  public class ValueTaskStepIteratorTest
  {
    private readonly PropertyBag ctx;
    private readonly ValueTaskStepIterator<IPropertyBag> subject;

    public ValueTaskStepIteratorTest()
    {
      this.ctx = new PropertyBag();
      this.ctx.Set<Tuple<int>>(Tuple.Create(0));
      this.subject = new ValueTaskStepIterator<IPropertyBag>();
    }

    public List<IValueTaskStep> CreateSyncSteps()
    {
      return new List<IValueTaskStep>
      {
        new CtxValueTaskStep<IPropertyBag>(asyncAction: null, syncAction: (ctx) =>
        {
          var idx =  ctx.Context.Get<Tuple<int>>().Item1 + 1;
          ctx.Context.Set<Tuple<int>>(Tuple.Create(idx));
        }, canProcessPredicate: () => true),
        new CtxValueTaskStep<IPropertyBag>(asyncAction: null, syncAction: (ctx) =>
        {
          var idx =  ctx.Context.Get<Tuple<int>>().Item1 + 1;
          ctx.Context.Set<Tuple<int>>(Tuple.Create(idx));
        }, canProcessPredicate: () => { return true; }),
      };
    }

    public List<IValueTaskStep> CreateAyncSteps()
    {
      var path = Path.Combine(Anixe.QualityTools.TestExample.GetExamplesDirPath(), "data.txt");
      return new List<IValueTaskStep>
      {
        new CtxValueTaskStep<IPropertyBag>(asyncAction: async (ctx, token) =>
        {
          var val = await File.ReadAllTextAsync(path);
          var idx =  ctx.Context.Get<Tuple<int>>().Item1 + int.Parse(val);
          ctx.Context.Set<Tuple<int>>(Tuple.Create(idx));
        }, syncAction: null, canProcessPredicate: () => true),
        new CtxValueTaskStep<IPropertyBag>(asyncAction: async (ctx, token) =>
        {
          var val = await File.ReadAllTextAsync(path);
          var idx =  ctx.Context.Get<Tuple<int>>().Item1 + int.Parse(val);
          ctx.Context.Set<Tuple<int>>(Tuple.Create(idx));
        }, syncAction: null, canProcessPredicate: () => true),
      };
    }

    public List<IValueTaskStep> CreateAyncSteps(CancellationTokenSource cts, bool mustProcess)
    {
      var path = Path.Combine(Anixe.QualityTools.TestExample.GetExamplesDirPath(), "data.txt");
      return new List<IValueTaskStep>
      {
        new CtxValueTaskStep<IPropertyBag>(asyncAction: async (ctx, token) =>
        {
          var val = await File.ReadAllTextAsync(path);
          var idx =  ctx.Context.Get<Tuple<int>>().Item1 + int.Parse(val);
          ctx.Context.Set<Tuple<int>>(Tuple.Create(idx));
          cts.Cancel();
        }, syncAction: null, canProcessPredicate: () => true),
        new CtxValueTaskStep<IPropertyBag>(asyncAction: async (ctx, token) =>
        {
          var val = await File.ReadAllTextAsync(path);
          var idx =  ctx.Context.Get<Tuple<int>>().Item1 + int.Parse(val);
          ctx.Context.Set<Tuple<int>>(Tuple.Create(idx));
        }, syncAction: null, canProcessPredicate: () => true) { MustProcessAfterCancel = mustProcess },
      };
    }

    public List<IValueTaskStep> CreateAyncStepsThrowEx()
    {
      var path = Path.Combine(Anixe.QualityTools.TestExample.GetExamplesDirPath(), "data.txt");
      return new List<IValueTaskStep>
      {
        new CtxValueTaskStep<IPropertyBag>(asyncAction: async (ctx, token) =>
        {
          var val = await File.ReadAllTextAsync("bllaaach");
          var idx =  ctx.Context.Get<Tuple<int>>().Item1 + int.Parse(val);
          ctx.Context.Set<Tuple<int>>(Tuple.Create(idx));
        }, syncAction: null, canProcessPredicate: () => true),
        new CtxValueTaskStep<IPropertyBag>(asyncAction: async (ctx, token) =>
        {
          var val = await File.ReadAllTextAsync(path);
          var idx =  ctx.Context.Get<Tuple<int>>().Item1 + int.Parse(val);
          ctx.Context.Set<Tuple<int>>(Tuple.Create(idx));
        }, syncAction: null, canProcessPredicate: () => true),
      };
    }

    [Fact]
    public async Task Should_IterateAllAsync_Over_All_Sync_Steps()
    {
      var steps = CreateSyncSteps();
      await this.subject.IterateAllAsync(this.ctx, steps, CancellationToken.None);
      Assert.True(steps.All(s => s.WasFired));
      Assert.Equal(2, ctx.Get<Tuple<int>>().Item1);
    }

    [Fact]
    public async Task Should_IterateAllAsync_Over_All_Async_Steps()
    {
      var steps = CreateAyncSteps();
      await this.subject.IterateAllAsync(this.ctx, steps, CancellationToken.None);
      Assert.True(steps.All(s => s.WasFired));
      Assert.Equal(4, ctx.Get<Tuple<int>>().Item1);
    }

    [Fact]
    public async Task Should_IterateAllAsync_Over_Async_Steps_Which_Must_Process_After_Cancel()
    {
      using (var cts = new CancellationTokenSource())
      {
        var steps = CreateAyncSteps(cts, mustProcess: true);
        await this.subject.IterateAllAsync(this.ctx, steps, cts.Token);
        Assert.True(steps.All(s => s.WasFired));
        Assert.Equal(4, ctx.Get<Tuple<int>>().Item1);
      }
    }

    [Fact]
    public async Task Should_IterateAllAsync_Over_Async_Steps_Which_Must_Not_Process_After_Cancel()
    {
      using (var cts = new CancellationTokenSource())
      {
        var steps = CreateAyncSteps(cts, mustProcess: false);
        await this.subject.IterateAllAsync(this.ctx, steps, cts.Token);
        Assert.Equal(1, steps.Count(s => s.WasFired));
        Assert.Equal(2, ctx.Get<Tuple<int>>().Item1);
      }
    }

    [Fact]
    public void Should_Throw_Exception_Which_Occurred_In_Step()
    {
      var steps = CreateAyncStepsThrowEx();
      Assert.ThrowsAsync<InvalidOperationException>(async () => await this.subject.IterateAllAsync(this.ctx, steps, CancellationToken.None));
    }
  }
}