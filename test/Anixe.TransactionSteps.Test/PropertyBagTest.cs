using System;
using Xunit;

namespace Anixe.TransactionSteps.Test
{
  public class PropertyBagTest
  {
    private readonly IPropertyBag subject;

    public PropertyBagTest()
    {
      this.subject = new PropertyBag();
    }

    [Fact]
    public void Should_Set_ReferenceType_Property()
    {
      var item = new Tuple<string, int>("test", 3);
      subject.Set<Tuple<string, int>>(item);

      var actual = subject.Get<Tuple<string, int>>();
      Assert.Same(item, actual);
    }

    [Fact]
    public void Should_Set_Property_Multiple_Times()
    {
      var item = new Tuple<string, int>("test", 3);
      subject.Set<Tuple<string, int>>(item);

      var item2 = new Tuple<string, int>("test2", 4);
      subject.Set<Tuple<string, int>>(item2);

      var actual = subject.Get<Tuple<string, int>>();
      Assert.Same(item2, actual);
    }

    [Fact]
    public void Should_Set_ValueType_Property()
    {
      var item = 3;
      subject.Set<int>(item);

      var actual = subject.Get<int>();
      Assert.Equal(item, actual);
    }
  }
}