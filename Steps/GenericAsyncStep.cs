using System;
using System.Threading;
using System.Threading.Tasks;

namespace Steps
{
  public class GenericAsyncStep : StepBase, IStep
  {
    private Action<IStep> action;

    public GenericAsyncStep(Action<IStep> action)
    {
      this.action = action;        
    }
    
    public bool CapProcess()
    {
        return true;
    }

    public bool IsAsync()
    {
        return true;
    }

    public void Process()
    {
      if(this.action != null)
      {
        this.action(this);
      }
    }

    public async Task ProcessAsync(CancellationToken token)
    {
      if(token.IsCancellationRequested)
      {
        var tcs = new TaskCompletionSource<int>();
        tcs.SetCanceled();
        await tcs.Task;        
      }
      await Task.Run(() => { this.action(this); }, token);
    }
  }
}