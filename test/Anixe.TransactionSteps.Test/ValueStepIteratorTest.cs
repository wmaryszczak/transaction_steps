using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anixe.TransactionSteps.Predefined;
using Xunit;

namespace Anixe.TransactionSteps.Test
{
  [Collection("Our Test Collection #1")]  
  public class ValueStepIteratorTest
  {
    private PropertyBag ctx;
    private ValueStepIterator<IPropertyBag> subject;

    public ValueStepIteratorTest()
    {
      this.ctx = new PropertyBag{};
      this.subject = new ValueStepIterator<IPropertyBag>(ctx);      
    }


    public LinkedList<IValueStep<IPropertyBag>> CreateValueSteps()
    {
      return new LinkedList<IValueStep<IPropertyBag>>
      (
        new List<IValueStep<IPropertyBag>>
        {
          new AsyncTestValueStep(),
          new AsyncTestValueStep(),
          new AsyncTestValueStep(),
        }
      );
    }

    [Fact]
    public async Task Should_Have_Low_Allocations()
    {
      Console.WriteLine($"GEN0 collections {GC.CollectionCount(0)}");
      var s = CreateValueSteps();
      var stats = new List<StepStatValue>(3);
      for (int i = 0; i < 100_000; i++)
      {
        var ctx = await this.subject.IterateAllAsync(null, s, null, stats, new CancellationToken());
      }
      Console.WriteLine($"GEN0 collections {GC.CollectionCount(0)}");
    }

  }
}