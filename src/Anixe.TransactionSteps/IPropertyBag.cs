namespace Anixe.TransactionSteps
{
  public interface IPropertyBag
  {
    bool Contains<T>();
    bool Contains<T>(string name);
    T Get<T>();
    T Get<T>(string name);
    void Set<T>(T property);
    void Set<T>(string name, T property);
    void Unset<T>();
    IPropertyBag Clone();
    IPropertyBag Clone(System.Type[] exclude);
  }  
}