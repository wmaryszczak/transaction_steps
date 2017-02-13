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
      this.typedProperties.Add(type, new PropertyBagItem<T>(property));
    }

    public override void Set<T>(string name, T property)
    {
      this.namedProperties.Add(name, new PropertyBagItem<T>(property));       
    }        
  }

  public class PropertyBag : IPropertyBag
  {
    protected class PropertyBagItem
    {
    }
    
    protected class PropertyBagItem<T> : PropertyBagItem
    {
      private readonly T instance;
      public PropertyBagItem(T instance)
      {
        this.instance = instance;
      }

      public T Value
      {
        get { return this.instance; }
      }
    }  
    
    protected readonly Dictionary<Type, PropertyBagItem> typedProperties;
    protected readonly Dictionary<string, PropertyBagItem> namedProperties;

    public PropertyBag()
    {
      this.typedProperties = new Dictionary<Type, PropertyBagItem>();
      this.namedProperties = new Dictionary<string, PropertyBagItem>();
    }

    private PropertyBag(Dictionary<Type, PropertyBagItem> typedProperties, Dictionary<string, PropertyBagItem> namedProperties)
    {
      this.typedProperties = new Dictionary<Type, PropertyBagItem>(typedProperties);
      this.namedProperties = new Dictionary<string, PropertyBagItem>(namedProperties);
    }

    public bool Contains<T>()
    {
      return TryGet<T>() != null;
    }

    public bool Contains<T>(string name)
    {
      return TryGet<T>(name) != null;
    }

    public virtual void Set<T>(T property)
    {
      var type = typeof(T);
      this.typedProperties[type] = new PropertyBagItem<T>(property);
    }    

    public virtual void Set<T>(string name, T property)
    {
      this.namedProperties[name] = new PropertyBagItem<T>(property);       
    }

    public T Get<T>()
    {
      var retval = TryGet<T>();
      if(retval != null)
      {
        return retval.Value;
      }
      return default(T);      
    }

    public T Get<T>(string name)
    {
      var retval = TryGet<T>(name);
      if(retval != null)
      {
        return retval.Value;
      }
      return default(T);      
    }

    public void Unset<T>()
    {
      var type = typeof(T);
      if(this.typedProperties.ContainsKey(type))
      {
        this.typedProperties.Remove(type);
      }      
    }

    public IPropertyBag Clone()
    {
      return new PropertyBag(this.typedProperties, this.namedProperties);
    }

    public IPropertyBag Clone(Type[] exclude)
    {
      var cloned = this.typedProperties.Where(kvp => !exclude.Any(item => item == kvp.Key)).ToDictionary(g => g.Key, g => g .Value);
      return new PropertyBag(
        cloned, 
        this.namedProperties
      );
    }

    private PropertyBagItem<T> TryGet<T>()
    {
      var type = typeof(T);
      PropertyBagItem instance = null; 
      if(this.typedProperties.TryGetValue(type, out instance))
      {
        return ((PropertyBagItem<T>)instance);
      }
      return null;
    }
         
    private PropertyBagItem<T> TryGet<T>(string name)
    {
      PropertyBagItem instance = null; 
      if(this.namedProperties.TryGetValue(name, out instance))
      {
        return ((PropertyBagItem<T>)instance);
      }
      return null;
    }
  }
}