using System;
using System.Collections.Generic;

namespace Anixe.TransactionSteps
{
  public abstract class StepBase
  {
    private string stepName;
    public IServiceProvider Services { get; set; }
    public string Name 
    { 
      get 
      {
        if(string.IsNullOrEmpty(this.stepName))
        {
          this.stepName = GetDefaultStepName();
        }
        return this.stepName;
      } 
      set
      {
        this.stepName = value;
      } 
    }
    public int TimeTaken { get; set; }
    public int ProcessedItemsCount { get; set; }    
    public LinkedList<IStep> Neighbourood { get; set; }
    public LinkedListNode<IStep> Current { get; set; }
    public bool WasFired { get; set; }
    public bool BreakProcessing { get; set; }    

    private string GetDefaultStepName()
    {
      return this.GetType().Name;
    }            
  }
}