using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace Anixe.TransactionSteps.Benchmark
{
  public class ValueTaskStepIteratorTest
  {
    private readonly List<IValueTaskStep> syncSteps;
    private readonly List<IValueTaskStep> asyncSteps;
    private readonly List<IValueTaskStep> allSteps;
    private readonly int ctx = 0;

    public ValueTaskStepIteratorTest()
    {
      this.syncSteps = CreateSyncSteps(20);
      this.asyncSteps = CreateAyncSteps(20);
      this.allSteps = new List<IValueTaskStep> { };
      this.allSteps.AddRange(this.syncSteps);
      this.allSteps.AddRange(this.asyncSteps);
    }

    private static List<IValueTaskStep> CreateSyncSteps(int stepCount)
    {
      var retval = new List<IValueTaskStep>(stepCount);
      for (int i = 0; i < stepCount; i++)
      {
        retval.Add(new ValueSyncStep() { Name = $"SyncStep{i}" });
      }
      return retval;
    }

    private static List<IValueTaskStep> CreateAyncSteps(int stepCount)
    {
      var retval = new List<IValueTaskStep>(stepCount);
      for (int i = 0; i < stepCount; i++)
      {
        retval.Add(new ValueAsyncStep() { Name = $"AsyncStep{i}" });
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
