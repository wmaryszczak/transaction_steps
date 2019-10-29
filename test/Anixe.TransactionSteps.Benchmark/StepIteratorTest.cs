using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anixe.TransactionSteps.Predefined;
using BenchmarkDotNet.Attributes;

namespace Anixe.TransactionSteps.Benchmark
{
  public class StepIteratorTest
  {
    private LinkedList<IStep> syncSteps;
    private LinkedList<IStep> asyncSteps;
    private LinkedList<IStep> allSteps;
    private IPropertyBag ctx;
    private IServiceProvider serviceProvider;

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


    private List<IStep> CreateSyncSteps(int stepCount)
    {
      var retval = new List<IStep>(stepCount);
      for (int i = 0; i < stepCount; i++)
      {
        retval.Add(new SyncStep() { Name = $"SyncStep{i}" });
      }
      return retval;
    }

    private List<IStep> CreateAyncSteps(int stepCount)
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