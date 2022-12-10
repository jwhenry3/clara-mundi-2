using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace ClaraMundi
{
    public class ItemTooltipUI : MonoBehaviour
    {
        ItemInstance ItemInstance;
        Item Item;
        public Image ItemImage;
        public TextMeshProUGUI ItemName;
        public TextMeshProUGUI ItemDescription;
        public Transform Column1;
        public Transform Column2;
        public ModificationUI ModificationUIPrefab;

        public static ItemTooltipUI Instance;

        private void Awake()
        {
            Instance = this;
        }

        public void SetItemInstance(ItemInstance itemInstance)
        {
            ItemInstance = itemInstance;
            Item = RepoManager.Instance.ItemRepo.GetItem(ItemInstance.ItemId);
            ItemName.text = Item.Name;
            ItemDescription.text = Item.Description;
            ItemImage.sprite = Item.Icon;
            ItemDescription.transform.parent.gameObject.SetActive(ItemDescription.text != "");
            UpdateModifications();
        }

        void UpdateModifications()
        {
            foreach (Transform child in Column1)
                Destroy(child.gameObject);
            foreach (Transform child in Column2)
                Destroy(child.gameObject);
            if (Item.Type == ItemType.Weapon || Item.Type == ItemType.Armor)
            {
                Column1.transform.parent.gameObject.SetActive(UpdateModificationList() > 0);
                return;
            }
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