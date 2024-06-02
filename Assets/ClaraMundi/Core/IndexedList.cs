using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClaraMundi
{
  [Serializable]
  public class IndexedList<K, V> where V : new()
  {
    private string KeyField;
    public List<V> Items = new();
    public Dictionary<K, V> IndexBy = new();

    public IndexedList(string keyField)
    {
      IndexBy = new();
      Items = new();
      KeyField = keyField;
    }

    public virtual bool FindPredicate(V item, K by)
    {

      if (item == null) return false;
      var type = item.GetType();
      if (type == null) return false;
      var property = type.GetProperty(KeyField);
      if (property == null) return false;
      var value = property.GetValue(item, null);
      return value.Equals(by);
    }

    public V Get(K by)
    {
      if (by == null) return default;
      if (!IndexBy.ContainsKey(by))
      {
        Items = Items ?? new();
        var value = Items.Find((v) => FindPredicate(v, by));
        if (value != null)
          return IndexBy[by] = value;
        else
          return Create(by);
      }
      return IndexBy[by];
    }

    public void Set(K by, V value)
    {
      var previous = Get(by);
      if (previous != null)
      {
        Items.Remove(previous);
      }
      if (!Items.Contains(value))
        Items.Add(value);
      IndexBy[by] = value;
    }

    public virtual V Create(K by)
    {
      Items = Items ?? new();
      var value = new V();
      Items.Add(value);
      return IndexBy[by] = value;
    }

  }
}