namespace Anixe.TransactionSteps
{
  public class StepStat
  {
    public string Name;
    public int TimeTaken;
    public int ProcessedItemsCount;

    public static StepStat CreateFromStep(IStep step)
    {
      return new StepStat
      {
        Name = step.Name,
        TimeTaken = step.TimeTaken,    
        ProcessedItemsCount = step.ProcessedItemsCount,        
      };
    }
  }
}