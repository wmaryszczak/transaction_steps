# transaction_steps

The idea is to reduce complexity of request processing by split into measurable steps, so:

* Do you have a lot of logic in the application?
* Do you have a deep dependency tree?
* Do you need to know how much time took any transaction part?

Lets make it flat and simple using transaction_steps

* Define a transaction made from steps or async steps

```c#
  public class StepsFactory
  {
    public static LinkedList<IStep> Create()
    {
     return new LinkedList<IStep>
     (
       new IStep[]
       {
         new LoadPackage(),
         new LoadStations(),
         new CreateSupplierRequests(),
         new SendRequestsToSuppliersAsync(),
         new ApplyCalculations(),
         new ApplyRuleEngineImplications(),
         new CreateTransactionResponse(),
       }
     ); 
    }
  }
```

* Pass step collection to iterator and run

```c#
  public class AnyTransaction
  {
    private readonly IServiceProvider services;

    public CarsTransaction(IServiceProvider services)
    {
      this.services = services;
    }

    public async Task<ResponseModel> ProcessAsync(RequestModel request)
    {
      var ctx = new PropertyBag();
      ctx.Set<RequestModel>(request);
      ctx.Set<ResponseModel>(new ResponseModel{});
      var steps = StepsFactory.Create();
      var errorHandler = new ProcessUnhandledErrors();

      var iterator = new StepIterator<IPropertyBag>(ctx);
      await iterator.IterateAllAsync(this.services, steps, errorHandler, new CancellationToken());
      return ctx.Get<ResponseModel>();
    }
  }  
```

* Stats can be collected

```c#
  var iterator = new StepIterator<IPropertyBag>(ctx);
  await iterator.IterateAllAsync(this.services, steps, errorHandler, new CancellationToken());

  var stats = iterator.Stats;
  LogIt(stats);
```

* Step can be synchronous 

```c#
  public class ApplyCalculations : SyncStepBase<IPropertyBag>, IStep<IPropertyBag>
  {
    public bool CanProcess()
    {
      //check if conditions to compute are valid
    }

    public void Process()
    {
      //compute something
    }
  }
```

* Step can be asynchronous
* Depenencies are resolved via well known IServiceProvider

```c#
  public class UpdateBookInSupplierAsync : AsyncStepBase<IPropertyBag>, IStep<IPropertyBag>
  {
    public bool CanProcess()
    {
      //check if conditions to compute are valid
    }

    public async Task ProcessAsync(CancellationToken token)
    {
      var httpReqest = CreateRequest();
      var client = this.Services.GetService(typeof(HttpClient)) as HttpClient;
      var httpResponse = await client.SendAsync(httpReqest);
      //do something more
    }
  }

```

* Steps share context

```c#
  public class UpdateBookInSupplierAsync : AsyncStepBase<IPropertyBag>, IStep<IPropertyBag>
  {
    public bool CanProcess()
    {
      //check if conditions to compute are valid
    }

    public async Task ProcessAsync(CancellationToken token)
    {
      var httpReqest = CreateRequest();
      var client = this.Services.GetService(typeof(HttpClient)) as HttpClient; //resolve service
      var httpResponse = await client.SendAsync(httpReqest);
      var response = await ReadResponseAsync(httpResponse);
      this.Context.Set<ResponseModel>(response);//set into context
    }
  }


  public class ApplyCalculations : SyncStepBase<IPropertyBag>, IStep<IPropertyBag>
  {
    public bool CanProcess()
    {
      //check if conditions to compute are valid
    }

    public void Process()
    {
      var response = this.Context.Get<ResponseModel>(); //get from context
      ApplyCalculations(response);
    }
  }  
```

* Step can be proxied

```c#
  public class StepsFactory
  {
    public static LinkedList<IStep> Create()
    {
     return new LinkedList<IStep>
     (
       new IStep[]
       {
         new ErrorLoggerProxy(new LoadPackage()),
         //more steps
       }
     ); 
    }
  }
```

* Step can extend the pipeline at runtime

```c#
  public class RegisterTransactionSubmodule : SyncStepBase<IPropertyBag>, IStep<IPropertyBag>
  {
    public bool CanProcess()
    {
      //check if conditions to process are valid
    }

    public void Process()
    {
      this.Neighbourood.AddAfter(this.Current, new ProcessAdditionalLogic());
      this.Neighbourood.AddLast(new SummarizeTransaction());
    }
  }  
```

## StepIteratorTest

|                                            Method |     Mean |     Error |    StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------------------------------------------- |---------:|----------:|----------:|-------:|------:|------:|----------:|
|  Should_Benchmark_IterateAllAsync_Over_Sync_Steps | 3.079 us | 0.0303 us | 0.0283 us | 0.2518 |     - |     - |   1.55 KB |
| Should_Benchmark_IterateAllAsync_Over_Async_Steps | 3.280 us | 0.0858 us | 0.0761 us | 0.4807 |     - |     - |   2.95 KB |
|   Should_Benchmark_IterateAllAsync_Over_All_Steps | 6.630 us | 0.0443 us | 0.0370 us | 0.6943 |     - |     - |   4.26 KB |


## ValueTaskStepIteratorTest

|                                            Method |     Mean |     Error |    StdDev |  Gen 0 |  Gen 1 | Gen 2 | Allocated |
|-------------------------------------------------- |---------:|----------:|----------:|-------:|-------:|------:|----------:|
|  Should_Benchmark_IterateAllAsync_Over_Sync_Steps | 2.754 us | 0.0218 us | 0.0193 us | 0.1755 |      - |     - |   1.09 KB |
| Should_Benchmark_IterateAllAsync_Over_Async_Steps | 3.095 us | 0.0261 us | 0.0231 us | 0.4044 |      - |     - |   2.49 KB |
|   Should_Benchmark_IterateAllAsync_Over_All_Steps | 6.175 us | 0.0448 us | 0.0397 us | 0.5569 | 0.0076 |     - |   3.43 KB |

## PropertyBagTest

|             Method |     Mean |     Error |    StdDev |   Median | Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------- |---------:|----------:|----------:|---------:|------:|------:|------:|----------:|
|     Set_Value_Type | 5.022 us | 0.6291 us | 1.7640 us | 5.300 us |     - |     - |     - |     856 B |
| Set_Reference_Type | 3.356 us | 0.2597 us | 0.7367 us | 2.900 us |     - |     - |     - |     136 B |
|     Get_Value_Type | 3.904 us | 0.2187 us | 0.6169 us | 4.050 us |     - |     - |     - |         - |
| Get_Reference_Type | 2.597 us | 0.1166 us | 0.3030 us | 2.500 us |     - |     - |     - |         - |
