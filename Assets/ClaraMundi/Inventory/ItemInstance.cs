using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ClaraMundi
{
    [Serializable]
    public class ItemInstance
    {
        public string OwnerId;
        public string StorageId;
        public string ItemInstanceId = Guid.NewGuid().ToString();
        public string ItemId;
        public int Quantity = 1;
        public bool IsEquipped;

    }
}