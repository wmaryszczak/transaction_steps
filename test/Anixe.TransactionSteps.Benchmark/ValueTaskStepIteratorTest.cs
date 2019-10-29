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
  public class ValueTaskStepIteratorTest
  {
    private List<IValueTaskStep> syncSteps;
    private List<IValueTaskStep> asyncSteps;
    private List<IValueTaskStep> allSteps;
    private int ctx;

    public ValueTaskStepIteratorTest()
    {
      this.syncSteps = CreateSyncSteps(20);
      this.asyncSteps = CreateAyncSteps(20);
      this.allSteps = new List<IValueTaskStep> { };
      this.allSteps.AddRange(this.syncSteps);
      this.allSteps.AddRange(this.asyncSteps);
    }


    private List<IValueTaskStep> CreateSyncSteps(int stepCount)
    {
      var retval = new List<IValueTaskStep>(stepCount);
      for (int i = 0; i < stepCount; i++)
      {
        retval.Add(new ValueSyncStep(){ Name = $"SyncStep{i}" });
      }
      return retval;
    }

    private List<IValueTaskStep> CreateAyncSteps(int stepCount)
    {
      var retval = new List<IValueTaskStep>(stepCount);
      for (int i = 0; i < stepCount; i++)
      {
        retval.Add(new ValueAsyncStep(){ Name = $"AsyncStep{i}" });
      }
      return retval;
    }


    [Benchmark]
    public async Task<List<StepStat>> Should_Benchmark_IterateAllAsync_Over_Sync_Steps()
    {
      var subject = new ValueTaskStepIterator<int>();
      var x = await subject.IterateAllAsync(this.ctx, this.syncSteps, CancellationToken.None);
      return x;
    }

    [Benchmark]
    public async Task<List<StepStat>> Should_Benchmark_IterateAllAsync_Over_Async_Steps()
    {
      var subject = new ValueTaskStepIterator<int>();
      var x = await subject.IterateAllAsync(this.ctx, this.asyncSteps, CancellationToken.None);
      return x;
    }

    [Benchmark]
    public async Task<List<StepStat>> Should_Benchmark_IterateAllAsync_Over_All_Steps()
    {
      var subject = new ValueTaskStepIterator<int>();
      var x = await subject.IterateAllAsync(this.ctx, this.allSteps, CancellationToken.None);
      return x;
    }
  }
}