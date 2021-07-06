using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace Anixe.TransactionSteps.Benchmark
{
  public class StepIteratorTest
  {
    private readonly LinkedList<IStep> syncSteps;
    private readonly LinkedList<IStep> asyncSteps;
    private readonly LinkedList<IStep> allSteps;
    private readonly IPropertyBag ctx;
    private readonly IServiceProvider serviceProvider = null;

    public StepIteratorTest()
    {
      this.ctx = new PropertyBag();
      this.syncSteps = new LinkedList<IStep>(CreateSyncSteps(20));
      this.asyncSteps = new LinkedList<IStep>(CreateAyncSteps(20));
      var tmp = new List<IStep> { };
      tmp.AddRange(this.syncSteps);
      tmp.AddRange(this.asyncSteps);
      this.allSteps = new LinkedList<IStep>(tmp);
    }

    private static List<IStep> CreateSyncSteps(int stepCount)
    {
      var retval = new List<IStep>(stepCount);
      for (int i = 0; i < stepCount; i++)
      {
        retval.Add(new SyncStep() { Name = $"SyncStep{i}" });
      }
      return retval;
    }

    private static List<IStep> CreateAyncSteps(int stepCount)
    {
      var retval = new List<IStep>(stepCount);
      for (int i = 0; i < stepCount; i++)
      {
        retval.Add(new AsyncStep() { Name = $"AsyncStep{i}" });
      }
      return retval;
    }

    [Benchmark]
    public async Task<IPropertyBag> Should_Benchmark_IterateAllAsync_Over_Sync_Steps()
    {
      var subject = new StepIterator<IPropertyBag>(ctx);
      var x = await subject.IterateAllAsync(this.serviceProvider, this.syncSteps, null, CancellationToken.None);
      return x;
    }

    [Benchmark]
    public async Task<IPropertyBag> Should_Benchmark_IterateAllAsync_Over_Async_Steps()
    {
      var subject = new StepIterator<IPropertyBag>(ctx);
      var x = await subject.IterateAllAsync(this.serviceProvider, this.asyncSteps, null, CancellationToken.None);
      return x;
    }

    [Benchmark]
    public async Task<IPropertyBag> Should_Benchmark_IterateAllAsync_Over_All_Steps()
    {
      var subject = new StepIterator<IPropertyBag>(ctx);
      var x = await subject.IterateAllAsync(this.serviceProvider, this.allSteps, null, CancellationToken.None);
      return x;
    }
  }
}
