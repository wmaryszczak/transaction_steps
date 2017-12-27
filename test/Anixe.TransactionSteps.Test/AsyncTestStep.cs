using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Anixe.TransactionSteps.Predefined;

namespace Anixe.TransactionSteps.Test
{
  public class AsyncTestStep : AsyncStepBase<IPropertyBag>, IStep<IPropertyBag>
  {
    public bool CanProcess()
    {
      return true;
    }

    public async Task<IPropertyBag> ProcessAsync(CancellationToken token)
    {
      return await ProcessAsyncInternal();
    }

    private async Task<IPropertyBag> ProcessAsyncInternal()
    {
      var pts = await Task<IPropertyBag>.Run(() => new List<CtxStep<IPropertyBag>> { new CtxStep<IPropertyBag>(DoSomething), new CtxStep<IPropertyBag>(DoSomething), new CtxStep<IPropertyBag>(DoSomething) });
      return this.Context;
    }

    private void DoSomething(IStep<IPropertyBag> obj)
    {
      var x = 3 + 3;
      obj.Context.Set(x);
    }
  }


  public class AsyncTestValueStep : AsyncValueStepBase<IPropertyBag>, IValueStep<IPropertyBag>
  {
    public bool CanProcess()
    {
      return true;
    }

    public ValueTask<IPropertyBag> ProcessAsync(CancellationToken token)
    {
      return new ValueTask<IPropertyBag>(ProcessAsyncInternal());
    }

    private async Task<IPropertyBag> ProcessAsyncInternal()
    {
      var pts = await Task<IPropertyBag>.Run(() => new List<CtxStep<IPropertyBag>> { new CtxStep<IPropertyBag>(DoSomething), new CtxStep<IPropertyBag>(DoSomething), new CtxStep<IPropertyBag>(DoSomething) });
      return this.Context;
    }

    private void DoSomething(IStep<IPropertyBag> obj)
    {
      var x = 3 + 3;
      obj.Context.Set(x);
    }
  }

}