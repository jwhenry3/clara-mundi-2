using System;

namespace ClaraMundi
{
    [Serializable]
    public class LootItem
    {
        public Item Item;
        public int Quantity;
        public float Chance = 1;
    }
}