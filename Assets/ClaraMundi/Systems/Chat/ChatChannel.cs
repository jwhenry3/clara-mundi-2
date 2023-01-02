using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace ClaraMundi
{
    // Hold the most recent messages to broadcast to the players who should receive them
    // each scene should have a channel and there should be channels that are global
    // like trade, lfg, etc
    // There could also be a system channel where all system messages go to reach all players
    public class ChatChannel : NetworkBehaviour
    {
        public Player player;
        public List<string> supportedChannels = new();

        [SyncVar(OnChange = nameof(OnMessage))]
        public ChatMessage LastMessage;

        private ChatMessage initialMessage;

        public override void OnStartServer()
        {
            base.OnStartServer();
            if (player == null)
            {
                foreach (var channel in supportedChannels)
                {
                    if (channel == "Say" || channel == "Shout") continue;
                    ChatManager.Instance.Channels[channel] = this;
                }
            }
            else
            {
                ChatManager.Instance.Channels[player.Character.name] = this;
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            initialMessage = LastMessage;
            if (player != null)
            {
                ChatManager.ReceivedMessage(new ChatMessage
                {
                    Type = ChatMessageType.System,
                    Message = $"Joined your Private Message Channel"
                });
                return;
            }

            foreach (var channel in supportedChannels)
            {
                ChatManager.ReceivedMessage(new ChatMessage
                {
                    Type = ChatMessageType.System,
                    Message = $"Joined the {channel} Channel"
                });
            }
        }

        private void OnDestroy()
        {
            if (player != null)
                ChatManager.Instance.Channels.Remove(player.Character.name);

            foreach (var channel in supportedChannels)
            {
                ChatManager.ReceivedMessage(new ChatMessage
                {
                    Type = ChatMessageType.System,
                    Message = $"Left the {channel} Channel"
                });
                if (channel == "Say" || channel == "Shout") continue;
                if (player != null) continue;
                ChatManager.Instance.Channels.Remove(channel);
            }
        }

        private void OnMessage(ChatMessage lastMessage, ChatMessage nextMessage, bool asServer)
        {
            if (asServer) return;
            if (lastMessage.MessageId == nextMessage.MessageId ||
                nextMessage.MessageId == initialMessage?.MessageId) return;

            if (nextMessage.Channel == "Say")
            {
                if (Vector3.Distance(PlayerManager.Instance.LocalPlayer.transform.position,
                        nextMessage.SenderPosition) > 50) return;
            }

            ChatManager.ReceivedMessage(nextMessage);
        }

        public void ServerSendMessage(ChatMessage message)
        {
            if (!IsServer) return;
            LastMessage = message;
        }
    }
}