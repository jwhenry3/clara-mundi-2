using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    public class ChatMessageUI : MonoBehaviour, IPointerMoveHandler, IPointerDownHandler
    {
        public string NodeId = StringUtils.UniqueId();
        TextMeshProUGUI Text;
        public ItemTooltipUI Tooltip => TooltipHandler.Instance.ItemTooltipUI;
        public ChatMessage ChatMessage;


        string sender;

        string color;

        private void Awake()
        {
            Text = GetComponent<TextMeshProUGUI>();
        }

        private void OnDestroy()
        {
            if (Tooltip.NodeId == NodeId)
                Tooltip.gameObject.SetActive(false);
        }

        public void SetChatMessage(ChatMessage message)
        {
            ChatMessage = message;
            UpdateColor();
            UpdateSenderStyle();
            SaveToText();
        }

        private void UpdateColor()
        {
            color = ChatManager.Instance.ChatConfiguration.GetColor(ChatMessage.Type, ChatMessage.Channel);
        }

        private void UpdateSenderStyle()
        {
            if (ChatMessage.Type == ChatMessageType.System)
            {
                sender = "";
                return;
            }

            var senderName = "System";
            var receiverName = "";
            if (!string.IsNullOrEmpty(ChatMessage.SenderCharacterName))
                senderName = Player.GetClickableName(ChatMessage.SenderCharacterName);
            if (!string.IsNullOrEmpty(ChatMessage.ToCharacterName))
                receiverName = Player.GetClickableName(ChatMessage.ToCharacterName);

            if (ChatMessage.Channel == "Whisper" &&
                ChatMessage.SenderCharacterName == null)
                sender = "[Whisper To] " + receiverName + ":";
            else if (ChatMessage.Channel == "Whisper")
                sender = "[Whisper From] " + senderName + ":";
            else if (ChatMessage.Channel == "Yell" && !string.IsNullOrEmpty(ChatMessage.SenderArea))
                sender = "[" + ChatMessage.Channel + "][" + ChatMessage.SenderArea + "] " + senderName + ":";
            else
                sender = "[" + ChatMessage.Channel + "] " + senderName + ":";
        }

        void SaveToText()
        {
            if (sender == "")
                Text.text = $"<color={color}>{ChatMessage.Message}</color>";
            else
                Text.text = $"<color={color}>{sender} {ChatMessage.Message}</color>";
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            var selectedLink = TextUtils.GetLinkUnder(Text, eventData);
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

        public void OnPointerDown(PointerEventData eventData)
        {
            var selectedLink = TextUtils.GetLinkUnder(Text, eventData);
            if (selectedLink == "") return;
            if (!selectedLink.Contains("player:")) return;
            if (eventData.button == PointerEventData.InputButton.Right)
                ChatWindowUI.Instance.OpenPlayerContextMenu(eventData, selectedLink.Substring(7));
        }

        private void ShowTooltip(string itemOrInstanceId)
        {
            ItemTooltipUtils.ShowTooltip(Tooltip, (RectTransform)transform, itemOrInstanceId);
        }
    }
}