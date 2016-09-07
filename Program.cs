using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Anixe.TransactionSteps;
using Anixe.TransactionSteps.Predefined;

namespace ConsoleApplication
{
  public class Program
  {
    public static void Main(string[] args)
    {
      MainAsync(args).Wait();
    }

    public static async Task MainAsync(string[] args)
    {
      Console.WriteLine("{0}: start", Task.CurrentId);
      var steps = new LinkedList<IStep>(
        new IStep[] {
          new Step((s) => { Console.Write("step1"); }),
          new Step((s) => 
          { 
            Console.WriteLine(" took {0} ms", s.Current.Previous.Value.TimeTaken);
            Console.WriteLine("step2");
            s.Neighbourood.AddAfter(s.Current, new Step((_) => { Console.WriteLine("step 2.1"); })); 
          }),
          new AsyncStep(DownloadDataAsync),
          new Step((s) => 
          { 
            Console.WriteLine("{0}->step5", s.Current.Previous.Value.Name); 
          })
        }
      );
      var ctx = new CancellationTokenSource();
      var iterator = new StepIterator();
      var stepsExecuted = await iterator.IterateAllAsync(null, steps, ctx.Token);

      PrintTimeTaken(steps);
      Console.WriteLine("{0}: finished {1} steps", Task.CurrentId, stepsExecuted);

    }

    private static async Task DownloadDataAsync(IStep current)
    {
        var cli = new HttpClient();
        using(var responseStream = await cli.GetStreamAsync("http://maps.googleapis.com/maps/api/geocode/json?address=San%20Francisco,%20CA&sensor=false"))
        using(var reader = new StreamReader(responseStream))
        {
          Console.WriteLine("{0}: {1}", Task.CurrentId, reader.ReadToEnd());
        }
    }

    private static void PrintTimeTaken(LinkedList<IStep> steps)
    {
      foreach(var step in steps)
      {
        Console.WriteLine("{0} took {1} ms", step.Name, step.TimeTaken.ToString());
      }
    }
  }
}
