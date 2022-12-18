using System;
using UnityEngine;
using System.Collections.Generic;

namespace ClaraMundi
{
    public class ChatManager : MonoBehaviour
    {
        public static event Action<ChatMessage> Messages;
        public readonly Dictionary<string, ChatChannel> Channels = new();
        public static ChatManager Instance;
        public ChatConfiguration ChatConfiguration;

        private void Awake()
        {
            if (Instance != null)
                Destroy(gameObject);
            else
                Instance = this;
        }

        public static void SendChatMessage(string channel, ChatMessage message)
        {
            if (PlayerManager.Instance.LocalPlayer)
                PlayerManager.Instance.LocalPlayer.Chat.SendMessage(channel, message);
        }

        public static void ReceivedMessage(ChatMessage message)
        {
            Messages?.Invoke(message);
        }
    }
}