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
        public ChannelType ChannelType;
        public Color Color;
        public string Format = "[sender]:";
    }
    public class ChatMessageUI : MonoBehaviour, IPointerMoveHandler, IPointerDownHandler
    {
        TextMeshProUGUI Text;
        public ItemTooltipUI Tooltip;
        public ChatMessage ChatMessage;
        [ShowInInspector]
        public ChatColor[] ChatColors;
        [ShowInInspector]
        public ChannelColor[] ChannelColors;


        string sender;

        string color;

        readonly Dictionary<ChannelType, Color> ChannelTypeDict = new();
        readonly Dictionary<ChatMessageType, Color> MessageTypeDict = new();
        readonly Dictionary<ChannelType, string> ChannelFormats = new();

        private void Awake()
        {
            Text = GetComponent<TextMeshProUGUI>();
            foreach (var c in ChannelColors)
            {
                ChannelTypeDict[c.ChannelType] = c.Color;
                ChannelFormats[c.ChannelType] = c.Format;
            }
            foreach (var c in ChatColors)
            {
                MessageTypeDict[c.MessageType] = c.Color;
            }
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
                    if (ChannelTypeDict.ContainsKey(ChatMessage.ChannelType))
                        color = "#" + ColorUtility.ToHtmlStringRGB(ChannelTypeDict[ChatMessage.ChannelType]);
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
                sender = "[System]:";
                return;
            }
            var senderName = "System";
            var receiverName = "";
            if (!string.IsNullOrEmpty(ChatMessage.SenderEntityId))
                senderName = Player.GetClickableName(ChatMessage.SenderEntityId);
            if (!string.IsNullOrEmpty(ChatMessage.ToEntityId))
                receiverName = Player.GetClickableName(ChatMessage.ToEntityId);
            if (!ChannelFormats.ContainsKey(ChatMessage.ChannelType))
            {
                Destroy(gameObject);
                return;
            }
            string[] formats = ChannelFormats[ChatMessage.ChannelType].Split(",");
            // use the second format instead of the first if the whisper was outgoing
            if (ChatMessage.ChannelType == ChannelType.Whisper && ChatMessage.SenderEntityId == PlayerManager.Instance.LocalPlayer.entityId)
                sender = formats[1].Replace("receiver", receiverName);
            else
                sender = formats[0].Replace("sender", senderName);
        }

        void SaveToText()
        {
            Text.text = $"<color={color}>{sender} {ChatMessage.Message}</color>";
        }
        public void OnPointerMove(PointerEventData eventData)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(Text, eventData.position, eventData.pressEventCamera);
            if (linkIndex == -1)
            {
                Tooltip.gameObject.SetActive(false);
                return;
            }
            var linkInfo = Text.textInfo.linkInfo[linkIndex];
            string selectedLink = linkInfo.GetLinkID();
            if (selectedLink != "")
            {
                if (selectedLink.Contains("item:"))
                {
                    ShowTooltip(selectedLink.Substring(5));
                    return;
                }
            }
            Tooltip.gameObject.SetActive(false);
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(Text, eventData.position, eventData.pressEventCamera);
            if (linkIndex == -1)
            {
                Tooltip.gameObject.SetActive(false);
                return;
            }
            var linkInfo = Text.textInfo.linkInfo[linkIndex];
            string selectedLink = linkInfo.GetLinkID();
            if (selectedLink == "") return;
            if (!selectedLink.Contains("player:")) return;
            if (eventData.button == PointerEventData.InputButton.Right)
                ChatWindowUI.Instance.OpenPlayerContextMenu(eventData, selectedLink.Substring(7));
        }

        private void ShowTooltip(string itemInstanceId)
        {
            if (!ItemManager.Instance.ItemsByInstanceId.ContainsKey(itemInstanceId)) return;
            ItemInstance instance = ItemManager.Instance.ItemsByInstanceId[itemInstanceId];
            Tooltip.SetItemInstance(instance);
            var position = transform.position;
            int horizontal = ScreenUtils.GetHorizontalWithMostSpace(position.x);
            int vertical = ScreenUtils.GetVerticalWithMostSpace(position.y);
            RectTransform thisRect = (RectTransform)transform;
            var transform1 = Tooltip.transform;
            RectTransform rect = (RectTransform)transform1;
            var rect1 = thisRect.rect;
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