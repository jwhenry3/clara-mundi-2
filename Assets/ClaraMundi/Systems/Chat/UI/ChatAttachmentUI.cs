using System;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    public class ChatAttachmentUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        public string NodeId = Guid.NewGuid().ToString();
        public string Key;
        public ItemTooltipUI Tooltip => TooltipHandler.Instance.ItemTooltipUI;
        public TextMeshProUGUI Text;

        public void Remove()
        {
            ChatWindowUI.Instance.RemoveAttachment(Key);
        }

        public void SetValue(string key, string value)
        {
            Key = key;
            Text.text = value;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            var selectedLink = Key;
            if (selectedLink.Contains("item:"))
            {
                if (Tooltip.NodeId == NodeId) return;
                Tooltip.NodeId = NodeId;
                ShowTooltip(selectedLink.Substring(5));
                return;
            }

            Tooltip.NodeId = null;
            Tooltip.gameObject.SetActive(false);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (Tooltip.NodeId == NodeId)
            {
                Tooltip.NodeId = null;
                Tooltip.gameObject.SetActive(false);
            }
        }

        private void ShowTooltip(string itemOrInstanceId)
        {
            ItemTooltipUtils.ShowTooltip(Tooltip, (RectTransform)transform, itemOrInstanceId);
        }
    }
}