using System;

namespace ClaraMundi
{
    public class ItemInstanceModel
    {
        public string StorageId;
        public string ItemInstanceId = Guid.NewGuid().ToString();
        public string ItemId;
        public int Quantity = 1;
        public bool IsEquipped;
    }
}