﻿namespace Anixe.TransactionSteps
{
  public abstract class ValueStepBase<T> : ValueStepBase
  {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public T Context { get; set; }
#pragma warning restore CS8618
  }
}
