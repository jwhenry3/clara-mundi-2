using FishNet.Object;
using UnityEngine;

namespace ClaraMundi
{
    public class ChatController : PlayerController
    {
        public PrivateMessageClient PrivateMessageClient;

        protected override void Awake()
        {
            base.Awake();
            PrivateMessageClient = GetComponent<PrivateMessageClient>();
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
                    PrivateMessageClient.SendMessage(message);
                    break;
                default:
                    SendMessageFromClient(channel, message);
                    break;
            }
        }

        [ServerRpc]
        private void SendMessageFromClient(string channel, ChatMessage message)
        {
            if (!IsServer) return;
            message.SenderCharacterName = player.Character.Name;
            ServerSendMessage(channel, message);
        }

        private void ServerSendMessage(string channel, ChatMessage message)
        {
            if (!IsServer) return;
            if (!ChatManager.Instance.Channels.ContainsKey(channel)) return;

            if (!string.IsNullOrEmpty(message.SenderCharacterName))
            {
                // track position for local channels
                message.SenderPosition =
                    EntityManager.Instance.Entities[message.SenderCharacterName].transform.position;
            }

            ChatManager.Instance.Channels[channel].ServerSendMessage(message);
        }
    }
}