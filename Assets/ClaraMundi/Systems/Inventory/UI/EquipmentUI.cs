using FishNet.Object.Synchronizing;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    public class EquipmentUI : PlayerUI, IPointerDownHandler
    {
        OwningEntityHolder owner = new();
        public ItemUI ItemNodePrefab;

        public Transform EquipmentContainer;
        public Transform RightHand;
        public Transform LeftHand;
        public Transform Head;
        public Transform Body;
        public Transform Hands;
        public Transform Legs;
        public Transform Feet;
        public Transform Back;
        public Transform Ring1;
        public Transform Ring2;
        public Transform Earring1;
        public Transform Earring2;
        public Transform Neck;

        public void OnEnable()
        {
            if (PlayerManager.Instance != null)
                OnPlayerChange(PlayerManager.Instance.LocalPlayer);
            Reload();
        }

        public void Reload()
        {
            Clear();
            Populate();
        }

        public void Clear()
        {
            foreach (ItemUI item in EquipmentContainer.GetComponentsInChildren<ItemUI>())
                Destroy(item.gameObject);
        }

        private Transform GetSlotContainer(string slotName)
        {
            return slotName switch
            {
                "right_hand" => RightHand,
                "left_hand" => LeftHand,
                "head" => Head,
                "body" => Body,
                "hands" => Hands,
                "legs" => Legs,
                "feet" => Feet,
                "back" => Back,
                "ring1" => Ring1,
                "ring2" => Ring2,
                "earring1" => Earring1,
                "earring2" => Earring2,
                "neck" => Neck,
                _ => null
            };
        }

        public void Populate()
        {
            if (player == null) return;
            foreach (var kvp in player.Equipment.EquippedItems)
                LoadItem(GetSlotContainer(kvp.Key), kvp.Value);
        }

        private void LoadItem(Transform container, int itemInstanceId)
        {
            if (container == null) return;
            // Remove extra ItemUI instances
            foreach (ItemUI child in container.GetComponentsInChildren<ItemUI>())
                child.ShowNoItem();
            if (itemInstanceId == 0) return;
            var instance = Instantiate(ItemNodePrefab, container, false);
            instance.transform.localPosition = Vector3.zero;
            instance.ShowEquippedStatus = false;
            instance.ItemInstanceId = itemInstanceId;
            instance.SetOwner(owner);
            instance.OnDoubleClick += OnUnequip;
        }

        protected override void OnPlayerChange(Player _player)
        {
            if (player != null)
            {
                player.Equipment.EquippedItems.OnChange -= OnEquipChange;
            }

            base.OnPlayerChange(_player);
            if (entity == null) return;
            owner.SetEntity(_player.Entity);
            Reload();
            player.Equipment.EquippedItems.OnChange += OnEquipChange;
        }

        public override void OnDestroy()
        {
            if (player != null && player.Equipment != null && player.Equipment.EquippedItems != null)
                player.Equipment.EquippedItems.OnChange -= OnEquipChange;
            base.OnDestroy();
        }

        private void OnEquipChange(SyncDictionaryOperation op, string key, int itemInstanceId, bool asServer)
        {
            if (asServer) return;
            Transform container = GetSlotContainer(key);
            if (container == null) return;
            LoadItem(container, itemInstanceId);
        }

        public void CloseContextMenu()
        {
            ContextMenuHandler.Instance.ContextualItem = null;
            ContextMenuHandler.Instance.ItemMenu.gameObject.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            SelectFirstItem();
            if (ContextMenuHandler.Instance.ContextualItem != null)
                CloseContextMenu();
        }

        public void OnUnequip(ItemUI item)
        {
            player.Inventory.UnequipItem(item.ItemInstance.ItemInstanceId);
            SelectFirstItem();
            CloseContextMenu();
        }

        public void SelectFirstItem()
        {
            var item = GetComponentInChildren<ItemUI>();
            if (item != null)
                EventSystem.current.SetSelectedGameObject(item.gameObject);
        }
    }
}