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
            var size = ScreenUtils.ActualSize(rectTransform);
            var rect2 = rect.rect;
            var verticalOffset = size.y / 2;
            if (vertical == -1)
                verticalOffset = rect2.height - (size.y / 2);

            transform1.position = new Vector3(
                position.x + (horizontal * (size.x / 2 + (rect2.width / 2) + 8)),
                position.y + verticalOffset,
                0
            );
            if (Tooltip.EquippedTooltip != null)
            {
                var position1 = transform1.position;
                Tooltip.EquippedTooltip.transform.position = new Vector3(
                    position1.x + (horizontal * 248),
                    position1.y ,
                    0
                );
            }

            Tooltip.gameObject.SetActive(true);
        }
    }
}