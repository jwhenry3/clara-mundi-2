using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    [Serializable]
    public class ChatColor
    {
        public ChatMessageType MessageType;
        public Color Color;
    }
    [Serializable]
    public class ChannelColor
    {
        public string Channel;
        public Color Color;
        public string Format = "[sender]:";
    }
    public class ChatMessageUI : MonoBehaviour, IPointerMoveHandler, IPointerDownHandler
    {
        public string NodeId = Guid.NewGuid().ToString();
        TextMeshProUGUI Text;
        public ItemTooltipUI Tooltip;
        public ChatMessage ChatMessage;
        [ShowInInspector]
        public ChatColor[] ChatColors;
        [ShowInInspector]
        public ChannelColor[] ChannelColors;


        string sender;

        string color;

        readonly Dictionary<string, Color> ChannelTypeDict = new();
        readonly Dictionary<ChatMessageType, Color> MessageTypeDict = new();

        private void Awake()
        {
            Text = GetComponent<TextMeshProUGUI>();
            foreach (var c in ChannelColors)
                ChannelTypeDict[c.Channel] = c.Color;
            foreach (var c in ChatColors)
                MessageTypeDict[c.MessageType] = c.Color;
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
            switch (ChatMessage.Type)
            {
                case ChatMessageType.Combat:
                    color = "#" + ColorUtility.ToHtmlStringRGB(MessageTypeDict[ChatMessageType.Combat]);
                    break;
                case ChatMessageType.Emote:
                    color = "#" + ColorUtility.ToHtmlStringRGB(MessageTypeDict[ChatMessageType.Emote]);
                    break;
                case ChatMessageType.System:
                    color = "#" + ColorUtility.ToHtmlStringRGB(MessageTypeDict[ChatMessageType.System]);
                    break;
                default:
                {
                    if (ChannelTypeDict.ContainsKey(ChatMessage.Channel))
                        color = "#" + ColorUtility.ToHtmlStringRGB(ChannelTypeDict[ChatMessage.Channel]);
                    else
                        color = "#ffffff";

                    break;
                }
            }
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
            if (!string.IsNullOrEmpty(ChatMessage.SenderEntityId))
                senderName = Player.GetClickableName(ChatMessage.SenderEntityId);
            if (!string.IsNullOrEmpty(ChatMessage.ToEntityId))
                receiverName = Player.GetClickableName(ChatMessage.ToEntityId);
            
            if (ChatMessage.Channel == "Whisper" &&
                ChatMessage.SenderEntityId == PlayerManager.Instance.LocalPlayer.entityId)
                sender = "[Whisper To] " + receiverName + ":";
            else if (ChatMessage.Channel == "Whisper")
                sender = "[Whisper From] " + senderName + ":";
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