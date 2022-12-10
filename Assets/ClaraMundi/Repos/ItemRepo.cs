using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ClaraMundi
{
    [CreateAssetMenu(fileName = "ItemRepo", menuName = "Clara Mundi/Inventory/ItemRepo")]
    [Serializable]
    public class ItemRepo : ScriptableObject
    {
        [HideInInspector]
        public Dictionary<string, Item> Items;

        public Item[] ItemList;

        public Item GetItem(string itemId)
        {
            return Items.ContainsKey(itemId) ? Items[itemId] : null;
        }
        
        public void OnEnable()
        {
            Items = new();
            if (ItemList == null) return;
            foreach (var item in ItemList)
            {
                Items.Add(item.ItemId, item);
            }
        }

    }
}