using System;
using System.Threading;
using System.Threading.Tasks;

namespace Steps
{
  public class GenericStep : StepBase, IStep
  {
    private Action<IStep> action;

    public GenericStep(Action<IStep> action)
    {
      this.action = action;        
    }
    
    public bool CapProcess()
    {
        return true;
    }

    public bool IsAsync()
    {
        return false;
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
      await Task.FromException(new NotImplementedException());
    }
  }
}