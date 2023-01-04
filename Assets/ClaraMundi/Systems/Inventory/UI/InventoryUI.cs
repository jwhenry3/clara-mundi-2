using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    public class InventoryUI : PlayerUI, IPointerDownHandler
    {
        readonly OwningEntityHolder owner = new();
        public ItemUI ItemNodePrefab;
        [HideInInspector] 
        public ItemUI ContextualItem;

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
                var parent = General;
                switch (item.Type)
                {
                    case ItemType.Armor:
                    case ItemType.Weapon:
                        parent = Equipment;
                        break;
                    case ItemType.Consumable:
                        parent = Consumables;
                        break;
                    case ItemType.KeyItem:
                        parent = QuestItems;
                        break;
                    case ItemType.Ingredient:
                    case ItemType.Generic:
                    default:
                        parent = General;
                        break;
                }
                var instance = Instantiate(ItemNodePrefab, parent, false);
                instance.ShowEquippedStatus = true;
                instance.ItemInstanceId = itemInstance.ItemInstanceId;
                instance.SetOwner(owner);
                instance.OnDoubleClick += OnUseOrEquipItem;
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
            EventSystem.current.SetSelectedGameObject(GetComponentInChildren<AutoFocus>().gameObject);
        }
        
        public void CloseContextMenu()
        {
            ContextMenuHandler.Instance.ContextualItem = null;
            ContextMenuHandler.Instance.ItemMenu.gameObject.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (ContextMenuHandler.Instance.ContextualItem != null)
            {
                EventSystem.current.SetSelectedGameObject(GetComponentInChildren<AutoFocus>().gameObject);
                CloseContextMenu();
            }
        }

        public void DropItem()
        {
            player.Inventory.DropItem(ContextMenuHandler.Instance.ContextualItem.ItemInstance.ItemInstanceId, 1);
            EventSystem.current.SetSelectedGameObject(GetComponentInChildren<AutoFocus>().gameObject);
            CloseContextMenu();
        }
        public void EquipItem()
        {
            player.Inventory.EquipItem(ContextMenuHandler.Instance.ContextualItem.ItemInstance.ItemInstanceId, true);
            EventSystem.current.SetSelectedGameObject(ContextMenuHandler.Instance.ContextualItem.gameObject);
            CloseContextMenu();
        }
        public void UseItem()
        {
            player.Inventory.UseItem(ContextMenuHandler.Instance.ContextualItem.ItemInstance.ItemInstanceId, 1);
            EventSystem.current.SetSelectedGameObject(ContextMenuHandler.Instance.ContextualItem.gameObject);
            CloseContextMenu();
        }
        public void LinkToChat()
        {
            ContextMenuHandler.Instance.ContextualItem.LinkToChat();
            EventSystem.current.SetSelectedGameObject(ContextMenuHandler.Instance.ContextualItem.gameObject);
            CloseContextMenu();
        }

    }
}
