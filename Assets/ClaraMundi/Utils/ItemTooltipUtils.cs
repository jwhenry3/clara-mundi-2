using UnityEngine;

namespace ClaraMundi
{
    public class ItemTooltipUtils
    {
        public static void ShowTooltip(ItemTooltipUI Tooltip, RectTransform rectTransform, string itemOrInstanceId)
        {
            if (!ItemManager.Instance.ItemsByInstanceId.ContainsKey(itemOrInstanceId))
                Tooltip.SetItem(RepoManager.Instance.ItemRepo.GetItem(itemOrInstanceId));
            else
                Tooltip.SetItemInstance(ItemManager.Instance.ItemsByInstanceId[itemOrInstanceId]);
            // If we cannot find an item to display, don't display
            if (Tooltip.Item == null)
            {
                Tooltip.gameObject.SetActive(false);
                return;
            }
            var position = rectTransform.position;
            int horizontal = ScreenUtils.GetHorizontalWithMostSpace(position.x);
            int vertical = ScreenUtils.GetVerticalWithMostSpace(position.y);
            var transform1 = Tooltip.transform;
            RectTransform rect = (RectTransform)transform1;
            var rect1 = rectTransform.rect;
            var rect2 = rect.rect;
            transform1.position = new Vector3(
                position.x + (horizontal * (rect1.width / 2 + (rect2.width / 2))),
                position.y + (vertical * (rect1.height / 2 + (rect2.height / 2))),
                0
            );
            Tooltip.gameObject.SetActive(true);
        }
    }
}