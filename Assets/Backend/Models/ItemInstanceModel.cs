using System;

namespace ClaraMundi
{
    public class ItemInstanceModel
    {
        public string StorageId;
        public string ItemInstanceId = StringUtils.UniqueId();
        public string ItemId;
        public int Quantity = 1;
        public bool IsEquipped;
    }
}