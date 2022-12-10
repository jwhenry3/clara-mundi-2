using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    public class InventoryUI : PlayerUI, IPointerDownHandler
    {
        readonly OwningEntityHolder owner = new();
        public ItemRepo ItemRepo => RepoManager.Instance.ItemRepo;
        public GameObject ContextMenu;
        public GameObject UseButton;
        public GameObject EquipButton;
        public GameObject DropButton;
        public GameObject SplitButton;
        public GameObject ItemsContainer;
        public ItemUI ItemNodePrefab;
        public ItemUI ContextualItem;


        private void Awake()
        {
            var t = ItemsContainer.transform;
            while (t.childCount > 0)
            {
                var child = t.GetChild(0);
                var node = child.GetComponent<ItemUI>();
                node.OnDoubleClick -= OnUseOrEquipItem;
                node.OnContextMenu -= OnContextMenu;
                Destroy(child.gameObject);
            }
            for (int i = 0; i < 80; i++)
            {
                var node = Instantiate(ItemNodePrefab, t, true);
                node.Position = i;
                node.SetOwner(owner);
                node.OnDoubleClick += OnUseOrEquipItem;
                node.OnContextMenu += OnContextMenu;
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
            }
        }
        public void OnContextMenu(ItemUI item, PointerEventData eventData)
        {
            ContextualItem = item;
            DropButton.SetActive(item.Item.Droppable);
            UseButton.SetActive(item.Item.Type == ItemType.Consumable);
            var equippable = item.Item.Type is ItemType.Armor or ItemType.Weapon;

            if (ContextualItem.ItemInstance.IsEquipped)
                EquipButton.GetComponentInChildren<TextMeshProUGUI>().text = "Unequip";
            else
                EquipButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
            EquipButton.SetActive(equippable);
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
            EventSystem.current.SetSelectedGameObject(ItemsContainer.transform.GetChild(0).gameObject, null);
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
