using FishNet.Object.Synchronizing;
using UnityEngine;

namespace ClaraMundi
{
    public class EquipmentItemUI : MonoBehaviour
    {
        public bool Active;
        ItemUI ItemUI;
        public bool IsWeaponSlot;
        public bool IsArmorSlot;
        public string EquipmentSlot;

        EquipmentController Equipment;

        private void Awake()
        {
            if (!Active) return;
            ItemUI = GetComponent<ItemUI>();
            ItemUI.EntityChange += OnEntityChange;
        }

        private void OnDestroy()
        {
            if (Equipment != null)
                Equipment.EquippedItems.OnChange -= OnEquipmentUpdate;
            if (ItemUI != null)
                ItemUI.EntityChange -= OnEntityChange;
        }
        public void Initialize()
        {
            if (!Active) return;
            if (Equipment != null)
            {
                Equipment.EquippedItems.TryGetValue(EquipmentSlot, out ItemUI.ItemInstanceId);
                ItemUI.updateQueued = true;
            }
        }
        void OnEntityChange()
        {
            if (Equipment != null)
                Equipment.EquippedItems.OnChange -= OnEquipmentUpdate;

            if (!Active) return;
            if (ItemUI.ItemStorage == null) return;

            Equipment = ItemUI.ItemStorage.GetComponent<EquipmentController>();
            if (Equipment != null)
                Equipment.EquippedItems.OnChange += OnEquipmentUpdate;
        }

        private void OnEquipmentUpdate(SyncDictionaryOperation op, string key, string itemInstanceId, bool asServer)
        {
            if (!Active) return;
            if (key != EquipmentSlot) return;
            ItemUI.ItemInstanceId = itemInstanceId;
            ItemUI.updateQueued = true;
        }
    }
}