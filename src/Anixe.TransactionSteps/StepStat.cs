namespace Anixe.TransactionSteps
{
  public class StepStat
  {
    public string Name;
    public double TimeTaken;
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

    public static StepStat CreateFromStep(IValueTaskStep step)
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
