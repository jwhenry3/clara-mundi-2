using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace ClaraMundi.Quests
{
    public class QuestRewardUI : MonoBehaviour
    {
        private LootItem _lootItem;

        public LootItem LootItem
        {
            get => _lootItem;
            set => SetLootItem(value);
        }

        public Image ItemImage;
        public TextMeshProUGUI ItemName;
        public TextMeshProUGUI Quantity;
        public TextMeshProUGUI Chance;


        private void SetLootItem(LootItem lootItem)
        {
            _lootItem = lootItem;
            gameObject.SetActive(_lootItem != null);
            if (_lootItem == null) return;
            ItemImage.sprite = lootItem.Item.Icon;
            ItemName.text = $"<color=#8af><link=\"item:{lootItem.Item.ItemId}\">{lootItem.Item.Name}</link></color>";
            Quantity.text = "x" + lootItem.Quantity;
            Chance.text = lootItem.Chance < 1 ? (lootItem.Chance * 100) + "%" : "";
        }
    }
}