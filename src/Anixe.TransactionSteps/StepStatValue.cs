namespace Anixe.TransactionSteps
{
  public struct StepStatValue
  {
    public string Name;
    public double TimeTaken;
    public int ProcessedItemsCount;

    public static StepStatValue CreateFromStep(IStep step)
    {
      return new StepStatValue
      {
        Name = step.Name,
        TimeTaken = step.TimeTaken,    
        ProcessedItemsCount = step.ProcessedItemsCount,        
      };
    }
  }
}