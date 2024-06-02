using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace ClaraMundi
{
  [CreateAssetMenu(fileName = "ItemRepo", menuName = "Clara Mundi/Inventory/ItemRepo")]
  [Serializable]
  public class ItemRepo : ScriptableObject
  {
    [HideInInspector]
    public Dictionary<string, Item> ItemsById;
    public Dictionary<string, Item> ItemsByName;

    public Item[] ItemList;

    public Item GetItem(string itemIdOrName)
    {
      if (ItemsById.ContainsKey(itemIdOrName))
        return ItemsById[itemIdOrName];
      if (ItemsByName.ContainsKey(itemIdOrName.ToLower().Replace(" ", "")))
        return ItemsByName[itemIdOrName.ToLower().Replace(" ", "")];
      return null;
    }

    public void OnEnable()
    {
      ItemsByName = new();
      ItemsById = new();
      if (ItemList == null) return;
      foreach (var item in ItemList)
      {
        ItemsById.Add(item.ItemId, item);
        ItemsByName.Add(item.Name.ToLower().Replace(" ", ""), item);
      }
    }

  }
}