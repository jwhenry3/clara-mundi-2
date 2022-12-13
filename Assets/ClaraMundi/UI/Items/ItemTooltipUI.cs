using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace ClaraMundi
{
    public class ItemTooltipUI : MonoBehaviour
    {
        public ItemInstance ItemInstance { get; private set; }
        Item Item;
        public string NodeId;
        public Image ItemImage;
        public TextMeshProUGUI ItemName;
        public TextMeshProUGUI ItemDescription;
        public TextMeshProUGUI EquippedStatus;
        public GameObject ModificationDivider;
        public Transform Column1;
        public Transform Column2;
        public ModificationUI ModificationUIPrefab;
        public ItemTooltipUI EquippedTooltip;


        public void SetItemInstance(ItemInstance itemInstance)
        {
            ItemInstance = itemInstance;
            Item = RepoManager.Instance.ItemRepo.GetItem(ItemInstance.ItemId);
            ItemName.text = Item.Name;
            ItemDescription.text = Item.Description;
            ItemImage.sprite = Item.Icon;
            EquippedStatus.gameObject.SetActive(ItemInstance.IsEquipped);
            ItemDescription.transform.parent.gameObject.SetActive(ItemDescription.text != "");
            UpdateModifications();
            UpdateEquipped();
        }

        public void UpdateEquipped()
        {
            if (EquippedTooltip == null) return;
            EquippedTooltip.gameObject.SetActive(false);
            if (!Item.Equippable) return;
            var equippedSlots = PlayerManager.Instance.LocalPlayer.Equipment.EquippedItems;
            var equipped = equippedSlots.ContainsKey(Item.EquipmentSlot)
                ? equippedSlots[Item.EquipmentSlot]
                : "";
            if (equipped == ItemInstance.ItemInstanceId) return;
            if (!string.IsNullOrEmpty(equipped))
                ShowEquippedTooltip(equipped);
        }
        void ShowEquippedTooltip(string itemInstanceId)
        {
            if (!ItemManager.Instance.ItemsByInstanceId.ContainsKey(itemInstanceId)) return;
            var equippedItemInstance = ItemManager.Instance.ItemsByInstanceId[itemInstanceId];
            EquippedTooltip.SetItemInstance(equippedItemInstance);
            EquippedTooltip.gameObject.SetActive(true);
        }

        void UpdateModifications()
        {
            foreach (Transform child in Column1)
                Destroy(child.gameObject);
            foreach (Transform child in Column2)
                Destroy(child.gameObject);
            if (Item.Type is ItemType.Weapon or ItemType.Armor)
            {
                bool hasModifications = UpdateModificationList() > 0;
                ModificationDivider.SetActive(hasModifications);
                Column1.transform.parent.gameObject.SetActive(hasModifications);
                return;
            }
            ModificationDivider.SetActive(false);
            Column1.transform.parent.gameObject.SetActive(false);
        }

        int UpdateModificationList()
        {
            var stats = Item.StatModifications;
            var attributes = Item.AttributeModifications;
            int count = 0;
            foreach (var stat in stats)
            {
                count++;
                var modificationUI = Instantiate(ModificationUIPrefab, count % 2 == 1 ? Column1 : Column2, true);
                modificationUI.SetValue(stat);
            }
            foreach (var attribute in attributes)
            {
                count++;
                var modificationUI = Instantiate(ModificationUIPrefab, count % 2 == 1 ? Column1 : Column2, true);
                modificationUI.SetValue(attribute);
            }
            return count;
        }
    }
}