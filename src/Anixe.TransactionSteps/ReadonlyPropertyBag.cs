namespace Anixe.TransactionSteps
{
  public class ReadonlyPropertyBag : PropertyBag
  {
    public override void Set<T>(T property)
    {
      var type = typeof(T);
      this.typedProperties.Add(type, property);
    }

    public override void Set<T>(string name, T property)
    {
      this.namedProperties.Add(name, property);
    }
  }
}
