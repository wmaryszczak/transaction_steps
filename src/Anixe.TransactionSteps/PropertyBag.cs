using System;
using System.Collections.Generic;
using System.Linq;

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

  public class PropertyBag : IPropertyBag
  {
    protected readonly Dictionary<Type, object> typedProperties;
    protected readonly Dictionary<string, object> namedProperties;

    public PropertyBag()
    {
      this.typedProperties = new Dictionary<Type, object>();
      this.namedProperties = new Dictionary<string, object>();
    }

    private PropertyBag(IEnumerable<KeyValuePair<Type, object>> typedProperties, Dictionary<string, object> namedProperties)
    {
      this.namedProperties = new Dictionary<string, object>(namedProperties);
      this.typedProperties = new Dictionary<Type, object>();
      foreach (var prop in typedProperties)
      {
        this.typedProperties.Add(prop.Key, prop.Value);
      }
    }

    public bool Contains<T>()
    {
      return this.typedProperties.ContainsKey(typeof(T));
    }

    public bool Contains<T>(string name)
    {
      return this.namedProperties.ContainsKey(name);
    }

    public virtual void Set<T>(T property)
    {
      var type = typeof(T);
      this.typedProperties[type] = property;
    }

    public virtual void Set<T>(string name, T property)
    {
      this.namedProperties[name] = property;
    }

    public T Get<T>()
    {
      var type = typeof(T);
      if (this.typedProperties.TryGetValue(type, out var item))
      {
        return (T)item;
      }
      return default;
    }

    public T Get<T>(string name)
    {
      if (this.namedProperties.TryGetValue(name, out var item))
      {
        return (T)item;
      }
      return default;
    }

    public void Unset<T>()
    {
      var type = typeof(T);
      this.typedProperties.Remove(type);
    }

    public IPropertyBag Clone()
    {
      return new PropertyBag(this.typedProperties, this.namedProperties);
    }

    public IPropertyBag Clone(Type[] exclude)
    {
      var cloned = this.typedProperties.Where(kvp => !Array.Exists(exclude, item => item == kvp.Key));
      return new PropertyBag(
        cloned,
        this.namedProperties
      );
    }
  }
}