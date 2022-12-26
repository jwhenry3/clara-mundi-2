using FishNet.Object;
using UnityEngine;

namespace ClaraMundi
{
    public class ChatController : PlayerController
    {
        public PrivateMessageClient PrivateMessageClient;
        public GlobalChatClient GlobalChatClient;
        public ChatChannel ChatChannel;
        protected override void Awake()
        {
            base.Awake();
            PrivateMessageClient = GetComponent<PrivateMessageClient>();
            GlobalChatClient = GetComponent<GlobalChatClient>();
        }

        public void SendMessage(string channel, ChatMessage message)
        {
            message.SenderCharacterName = null;
            if (!string.IsNullOrEmpty(message.ToCharacterName) && message.Channel == "Whisper")
            {
                // send the chat message to the chat window even though this is outgoing and not incoming
                // the private channel on the to-entity will not send the message to the sender, so we must
                // display it client-side only
                ChatManager.ReceivedMessage(message);
            }

            message.SenderCharacterName = player.Character.Name;
            switch (channel)
            {
                case "Party":
                    player.Party.SendMessage(message);
                    break;
                case "Whisper":
                    PrivateMessageClient.SendChatMessage(message);
                    break;
                case "Trade":
                case "LFG":
                case "Yell":
                    GlobalChatClient.SendChatMessage(channel, message);
                    break;
                case "Say":
                case "Shout":
                    SendMessageFromClient(channel, message);
                    break;
            }
        }

        [ServerRpc]
        private void SendMessageFromClient(string channel, ChatMessage message)
        {
            if (!IsServer) return;
            if (channel != "Say" && channel != "Shout") return;
            message.SenderCharacterName = player.Character.Name;
            ServerSendMessage(channel, message);
        }

        private void ServerSendMessage(string channel, ChatMessage message)
        {
            if (!IsServer) return;
            if (channel != "Say" && channel != "Shout") return;
            if (player == null) return;
            if (!ChatManager.Instance.Channels.ContainsKey(player.gameObject.scene.name)) return;
            var chatChannel = ChatManager.Instance.Channels[player.gameObject.scene.name];
            message.SenderPosition = player.transform.position;
            chatChannel.ServerSendMessage(message);
        }
    }
}