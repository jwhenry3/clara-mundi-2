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
        public GameObject ContextMenu;
        public ItemUI ItemNodePrefab;
        public ItemUI ContextualItem;

        public Transform Equipment;
        public Transform Consumables;
        public Transform General;
        public Transform QuestItems;

        public GameObject DropButton;
        public GameObject UseButton;
        public GameObject EquipButton;
        public GameObject SplitButton;


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
                instance.SetOwner(owner);
                instance.Tooltip = ItemTooltipUI;
                instance.ItemInstance = itemInstance;
                instance.Initialize();
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
            ContextualItem = item;
            DropButton.SetActive(item.Item.Droppable);
            UseButton.SetActive(item.Item.Type == ItemType.Consumable);
            var isEquipped = ContextualItem.ItemInstance.IsEquipped;
            EquipButton.GetComponentInChildren<TextMeshProUGUI>().text = isEquipped? "Unequip" : "Equip";
            EquipButton.SetActive(item.Item.Equippable);
            SplitButton.SetActive(ContextualItem.ItemInstance.Quantity > 1);
            ContextMenu.SetActive(true);
            ContextMenu.transform.position = eventData.position;
        }
        public void CloseContextMenu()
        {
            ContextualItem = null;
            ContextMenu.SetActive(false);
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
    }
}
