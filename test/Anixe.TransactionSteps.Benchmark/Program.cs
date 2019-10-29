using System;
using Anixe.QualityTools.Benchmark;
using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Validators;

namespace Anixe.TransactionSteps.Benchmark
{
  class Program
  {
    static void Main(string[] args)
    {
      var config = ManualConfig.CreateEmpty()
        .With(ConsoleLogger.Default)
        .With(DefaultColumnProviders.Instance)
        .With(MemoryDiagnoser.Default)
        .With(EnvironmentAnalyser.Default,
              OutliersAnalyser.Default,
              MinIterationTimeAnalyser.Default,
              MultimodalDistributionAnalyzer.Default,
              RuntimeErrorAnalyser.Default)
        .With(BaselineValidator.FailOnError,
              SetupCleanupValidator.FailOnError,
              JitOptimizationsValidator.FailOnError,
              RunModeValidator.FailOnError)
        .With(Job.Core.With(new GcMode()
        {
          Force = false // tell BenchmarkDotNet not to force GC collections after every iteration
        }));

      new BenchmarkRunner("My Application Tests").Run(args, config);
    }
  }
}