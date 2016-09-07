using System;
using System.Threading;
using System.Threading.Tasks;

namespace Anixe.TransactionSteps
{
  public class GenericStep : StepBase, IStep
  {
    private Action<IStep> action;
    private Predicate<IStep> canProcessPredicate;

    public GenericStep(Action<IStep> action, Predicate<IStep> canProcessPredicate = null)
    {
      this.action = action;        
      this.canProcessPredicate = canProcessPredicate ?? True;
    }

    public bool CanProcess()
    {
        return this.canProcessPredicate(this);
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

    private bool True(IStep step)
    {
      return true;
    }    
  }
}