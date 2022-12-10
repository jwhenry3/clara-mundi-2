using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{

    public class ChatWindowUI : MonoBehaviour
    {
        public static ChatWindowUI Instance;
        public ItemTooltipUI Tooltip;
        public Transform ChatMessageContainer;
        public Transform AttachmentsContainer;
        public ChatAttachmentUI AttachmentPrefab;
        public ChatMessageUI ChatMessagePrefab;

        public TextMeshProUGUI ChannelText;

        public GameObject PlayerContextMenu;

        Dictionary<string, string> MessageAttachments = new();

        public TMP_InputField RecipientField;
        public TMP_InputField InputField;
        public Transform SocialContainer;
        public Transform CombatContainer;
        public Transform SystemContainer;

        Player ContextualPlayer;

        string channel = "Say";

        private void Awake()
        {
            Instance = this;
            ChatManager.Messages += OnMessage;
        }

        private void OnDestroy()
        {
            ChatManager.Messages -= OnMessage;
        }

        private void OnMessage(ChatMessage message)
        {
            ClearOutOfBounds();
            
            var instance = Instantiate(ChatMessagePrefab);
            instance.Tooltip = Tooltip;
            instance.SetChatMessage(message);
            instance.transform.SetParent(ChatMessageContainer);
            if (ChatMessageContainer.childCount > 100)
                Destroy(ChatMessageContainer.GetChild(0).gameObject);
            // Add message also to different tabs that take specific types of messages
            // this allows for filtering out unwanted messages if they are looking for specific 
            // kinds
            var container = message.Type switch
            {
                ChatMessageType.Combat => CombatContainer,
                ChatMessageType.System => SystemContainer,
                _ => SocialContainer
            };
            
            instance = Instantiate(ChatMessagePrefab);
            instance.Tooltip = Tooltip;
            instance.SetChatMessage(message);
            instance.transform.SetParent(container);
            if (container.childCount > 100)
                Destroy(container.GetChild(0).gameObject);
        }

        void ClearOutOfBounds()
        {
            if (ChatMessageContainer.childCount > 99)
            {
                // remove the oldest first
                Destroy(ChatMessageContainer.GetChild(0));
                // recurse until no longer out of bounds
                ClearOutOfBounds();
            }
        }

        public void AddItemLink(ItemInstance itemInstance)
        {
            if (MessageAttachments.ContainsKey("item:" + itemInstance.ItemInstanceId))
                return;
            Item item = RepoManager.Instance.ItemRepo.GetItem(itemInstance.ItemId);
            string itemLink = $"<nobr><color=#88aaff><link=\"item:{itemInstance.ItemInstanceId}\">{item.Name}</link></color></nobr>";
            MessageAttachments.Add("item:" + itemInstance.ItemInstanceId, itemLink);
            UpdateAttachmentList();
        }
        public void RemoveAttachment(string key)
        {
            if (MessageAttachments.ContainsKey(key))
            {
                MessageAttachments.Remove(key);
                UpdateAttachmentList();
            }
        }

        void UpdateAttachmentList()
        {
            List<string> kept = new();
            foreach (Transform child in AttachmentsContainer)
            {
                var instance = child.GetComponent<ChatAttachmentUI>();
                if (!MessageAttachments.ContainsKey(instance.Key))
                    Destroy(child.gameObject);
                else
                    kept.Add(instance.Key);
            }
            foreach (var kvp in MessageAttachments)
            {
                if (kept.Contains(kvp.Key)) continue;
                ChatAttachmentUI instance = Instantiate(AttachmentPrefab, AttachmentsContainer, true);
                instance.SetValue(kvp.Key, kvp.Value);
            }
        }

        public void SendChatMessage()
        {
            if (!string.IsNullOrEmpty(InputField.text))
            {
                string additionalText = "";
                if (MessageAttachments.Count > 0)
                {
                    additionalText += " [";
                    int count = 0;
                    foreach (var kvp in MessageAttachments)
                    {
                        if (count > 0)
                            additionalText += ", ";
                        additionalText += kvp.Value;
                        count++;
                    }
                    additionalText += "]";
                }
                string channelName = channel;
                string toEntityId = "";
                if (channel == "Whisper")
                {
                    if (PlayerManager.Instance.PlayersByName.TryGetValue(RecipientField.text.ToLower(), out var player))
                    {
                        channelName = "player:" + RecipientField.text;
                        toEntityId = player.entityId;
                    }
                }
                ChatManager.SendChatMessage(channelName, new ChatMessage
                {
                    SenderEntityId = PlayerManager.Instance.LocalPlayer.entityId,
                    Type = ChatMessageType.Chat,
                    Message = InputField.text + additionalText,
                    ToEntityId = toEntityId,
                });
                InputField.text = "";
                MessageAttachments = new();
                UpdateAttachmentList();
            }
        }

        public void OpenPlayerContextMenu(PointerEventData eventData, string playerEntityId)
        {
            if (!PlayerManager.Instance.Players.TryGetValue(playerEntityId, out ContextualPlayer)) return;
            PlayerContextMenu.transform.position = eventData.position;
            PlayerContextMenu.SetActive(true);
        }
        public void ClosePlayerContextMenu()
        {
            ContextualPlayer = null;
            PlayerContextMenu.SetActive(false);
        }

        void SetChannel(string channelName)
        {
            ChannelText.text = channelName;
            channel = channelName;
            RecipientField.gameObject.SetActive(channelName == "Whisper");

        }

        public void SetSayChannel()
        {
            SetChannel("Say");
        }

        public void SetWhisperChannel()
        {
            SetChannel("Whisper");
        }

        public void SetShoutChannel()
        {
            SetChannel("Shout");
        }
        public void SetYellChannel()
        {
            SetChannel("Yell");
        }

        public void SetTradeChannel()
        {
            SetChannel("Trade");
        }

        public void SetLFGChannel()
        {
            SetChannel("LFG");
        }
        public void SetPartyChannel()
        {
            SetChannel("Party");
        }
    }
}