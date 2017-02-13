# transaction_steps

The idea is to reduce complexity of request processing by split into measurable steps, so:

* Do you have a lot of logic in the application?
* Do you have a deep dependency tree?
* Do you need to know how much time took any transaction part?

Lets make it flat and simple using transaction_steps

* Define a transaction made from steps or async steps

```
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

```
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

```
  var iterator = new StepIterator<IPropertyBag>(ctx);
  await iterator.IterateAllAsync(this.services, steps, errorHandler, new CancellationToken());

  var stats = iterator.Stats;
  LogIt(stats);
```

* Step can be synchronous 

```
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

```
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

```
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

```
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

```
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
