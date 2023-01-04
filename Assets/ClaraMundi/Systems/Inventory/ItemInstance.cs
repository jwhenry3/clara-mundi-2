using System;

namespace ClaraMundi
{
    [Serializable]
    public class ItemInstance
    {
        // runtime id to identify the instance
        public int ItemInstanceId;
        
        public string StorageId;
        public string CharacterId;
        
        public string ItemId;
        
        public int Quantity;
        public bool IsEquipped;
    }
}