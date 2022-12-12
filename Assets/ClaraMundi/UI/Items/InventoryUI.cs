using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace ClaraMundi
{
    public class InventoryUI : PlayerUI, IPointerDownHandler
    {
        readonly OwningEntityHolder owner = new();
        public ItemRepo ItemRepo => RepoManager.Instance.ItemRepo;
        public ItemTooltipUI ItemTooltipUI;
        public ContextMenu ContextMenu;
        public ItemUI ItemNodePrefab;
        [HideInInspector]
        public ItemUI ContextualItem;

        public RectTransform EquipmentImage;
        public RectTransform ConsumablesImage;
        public RectTransform GeneralImage;
        public RectTransform QuestItemsImage;
        
        public Transform Equipment;
        public Transform Consumables;
        public Transform General;
        public Transform QuestItems;


        private void Awake()
        {
            Reload();
        }

        private void OnEnable()
        {
            Reload();
        }

        public void Reload()
        {
            CleanUp();
            Populate();
        }
        private void CleanUp()
        {
            foreach (Transform child in Equipment.transform)
                Destroy(child.gameObject);
            foreach (Transform child in Consumables.transform)
                Destroy(child.gameObject);
            foreach (Transform child in General.transform)
                Destroy(child.gameObject);
            foreach (Transform child in QuestItems.transform)
                Destroy(child.gameObject);
        }

        private void Populate()
        {
            if (player == null) return;
            if (player.Inventory == null) return;
            if (player.Inventory.ItemStorage == null) return;
            foreach (var kvp in player.Inventory.ItemStorage.PrivateItems)
            {
                var itemInstance = kvp.Value;
                var item = RepoManager.Instance.ItemRepo.GetItem(itemInstance.ItemId);
                var instance = Instantiate(ItemNodePrefab);
                instance.ShowEquippedStatus = true;
                instance.ItemInstance = itemInstance;
                instance.SetOwner(owner);
                instance.Tooltip = ItemTooltipUI;
                instance.Initialize();
                instance.OnDoubleClick += OnUseOrEquipItem;
                instance.OnContextMenu += OnContextMenu;
                switch (item.Type)
                {
                    case ItemType.Armor:
                    case ItemType.Weapon:
                        instance.transform.SetParent(Equipment);
                        break;
                    case ItemType.Consumable:
                        instance.transform.SetParent(Consumables);
                        break;
                    case ItemType.Ingredient:
                    case ItemType.Generic:
                        instance.transform.SetParent(General);
                        break;
                    case ItemType.KeyItem:
                        instance.transform.SetParent(QuestItems);
                        break;
                    default:
                        instance.transform.SetParent(General);
                        break;
                }
            }
        }
        protected override void OnPlayerChange(Player _player)
        {
            base.OnPlayerChange(_player);
            if (entity == null) return;
            owner.SetEntity(entity);
            Reload();
        }

        public void OnUseOrEquipItem(ItemUI item)
        {
            switch (item.Item.Type)
            {
                case ItemType.Armor:
                case ItemType.Weapon:
                    player.Inventory.EquipItem(item.ItemInstance.ItemInstanceId, true);
                    break;
                case ItemType.Consumable:
                    player.Inventory.UseItem(item.ItemInstance.ItemInstanceId, 1);
                    break;
                default:
                    return;
            }
        }
        public void OnContextMenu(ItemUI item, PointerEventData eventData)
        {
            if (eventData == null)
            {
                if (ContextualItem == item)
                    CloseContextMenu();
                if (ItemTooltipUI)
                    ItemTooltipUI.gameObject.SetActive(false);
                return;
            }
            ContextualItem = item;
            ContextMenu.SetItemActive("Drop", item.Item.Droppable);
            ContextMenu.SetItemActive("Use", item.Item.Type == ItemType.Consumable);
            var isEquipped = ContextualItem.ItemInstance.IsEquipped;
            ContextMenu.ChangeLabelOf("Equip", isEquipped? "Unequip" : "Equip");
            ContextMenu.SetItemActive("Equip", item.Item.Equippable);
            ContextMenu.SetItemActive("Split", ContextualItem.ItemInstance.Quantity > 1);
            ContextMenu.gameObject.SetActive(true);
            ContextMenu.transform.position = eventData.position;
        }
        public void CloseContextMenu()
        {
            ContextualItem = null;
            ContextMenu.gameObject.SetActive(false);
            ContextMenu.transform.localPosition = new Vector2(0, 0);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (ContextualItem != null)
            {
                CloseContextMenu();
            }
        }

        public void DropItem()
        {
            player.Inventory.DropItem(ContextualItem.ItemInstance.ItemInstanceId, 1);
            CloseContextMenu();
        }
        public void EquipItem()
        {
            player.Inventory.EquipItem(ContextualItem.ItemInstance.ItemInstanceId, true);
            CloseContextMenu();
        }
        public void UseItem()
        {
            player.Inventory.UseItem(ContextualItem.ItemInstance.ItemInstanceId, 1);
            CloseContextMenu();
        }
        public void LinkToChat()
        {
            ContextualItem.LinkToChat();
            CloseContextMenu();
        }

        public void SetTabActive(string tab)
        {
            var active = new Vector3(1.5f, 1.5f, 1);
            var inactive = new Vector3(1, 1, 1);
            EquipmentImage.localScale = tab == "Equipment" ? active : inactive;
            ConsumablesImage.localScale = tab == "Consumables" ? active : inactive;
            GeneralImage.localScale = tab == "General" ? active : inactive;
            QuestItemsImage.localScale = tab == "QuestItems" ? active : inactive;
        }
        
    }
}
