using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Steps;

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
      var steps = new LinkedList<IStep>(
        new IStep[] {
          new GenericStep((s) => { Console.Write("step1"); }),
          new GenericStep((s) => 
          { 
            Console.WriteLine(" took {0} ms", s.Current.Previous.Value.TimeTaken);
            Console.WriteLine("step2");
            s.Neighbourood.AddAfter(s.Current, new GenericStep((_) => { Console.WriteLine("step 2.1"); })); 
          }),
          new GenericStep((s) => { Console.WriteLine("step5"); })
        }
      );
      var ctx = new CancellationTokenSource();
      var iterator = new StepIterator();
      await iterator.IterateAllAsync(null, steps, ctx.Token);
    }
  }
}
