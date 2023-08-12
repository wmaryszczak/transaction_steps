using System;
using System.Diagnostics;

namespace Anixe.TransactionSteps.Helpers;

internal static class StopwatchHelper
{
  private const long TicksPerMillisecond = 10000;
  private const long TicksPerSecond = TicksPerMillisecond * 1000;
  private static readonly double TickFrequency = (double)TicksPerSecond / Stopwatch.Frequency;

  // this method exists in Stopwatch class since .NET 7
  public static TimeSpan GetElapsedTime(long startingTimestamp)
  {
    return GetElapsedTime(startingTimestamp, Stopwatch.GetTimestamp());
  }

  // this method exists in Stopwatch class since .NET 7
  public static TimeSpan GetElapsedTime(long startingTimestamp, long endingTimestamp)
  {
    return new TimeSpan((long)((endingTimestamp - startingTimestamp) * TickFrequency));
  }
}
