using BenchmarkDotNet.Attributes;
using System;

namespace Anixe.TransactionSteps.Benchmark
{
  public class PropertyBagTest
  {
    private IPropertyBag subject;
    private object obj = new object();

    [IterationSetup]
    public void IterationSetup()
      => this.subject = new PropertyBag();

    [IterationSetup(Target = nameof(Get_Value_Type))]
    public void Get_Value_Type_Setup()
    {
      IterationSetup();
      this.subject.Set<int>(1);
    }

    [IterationSetup(Target = nameof(Get_Reference_Type))]
    public void Get_Reference_Type_Setup()
    {
      IterationSetup();
      this.subject.Set<string>("1");
    }

    [Benchmark]
    public void Set_Value_Type()
    {
      for (int i = 0; i < 30; i++)
      {
        subject.Set<int>(i);
      }
    }

    [Benchmark]
    public void Set_Reference_Type()
    {
      for (int i = 0; i < 30; i++)
      {
        subject.Set<object>(this.obj);
      }
    }

    [Benchmark]
    public void Get_Value_Type()
    {
      for (int i = 0; i < 30; i++)
      {
        subject.Get<int>();
      }
    }

    [Benchmark]
    public void Get_Reference_Type()
    {
      for (int i = 0; i < 30; i++)
      {
        subject.Get<string>();
      }
    }
  }
}