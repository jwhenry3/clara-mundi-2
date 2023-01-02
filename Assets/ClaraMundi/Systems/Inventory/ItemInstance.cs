using System;

namespace ClaraMundi
{
    [Serializable]
    public class ItemInstance 
    {
        public string CharacterId;
        public string ItemId;
        public string ItemInstanceId;
        public int Quantity;
        public string StorageId;

        public bool IsEquipped;

    }
}