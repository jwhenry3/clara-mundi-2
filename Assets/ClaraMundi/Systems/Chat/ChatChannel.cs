using System.Collections.Generic;
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

        [SyncVar(OnChange = nameof(OnMessage), ReadPermissions = ReadPermission.OwnerOnly)]
        public ChatMessage LastPrivateMessage;

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
                if (IsOwner)
                {
                    ChatManager.ReceivedMessage(new ChatMessage
                    {
                        Type = ChatMessageType.System,
                        Message = $"Joined your Private Message Channel"
                    });
                }

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
            {
                if (!IsServer) return;
                ChatManager.Instance.Channels.Remove(player.Character.name);
                return;
            }

            foreach (var channel in supportedChannels)
            {
                ChatManager.ReceivedMessage(new ChatMessage
                {
                    Type = ChatMessageType.System,
                    Message = $"Left the {channel} Channel"
                });
                if (channel == "Say" || channel == "Shout") continue;
                if (!IsServer) continue;
                ChatManager.Instance.Channels.Remove(channel);
            }
        }

        private void OnMessage(ChatMessage lastMessage, ChatMessage nextMessage, bool asServer)
        {
            if (asServer) return;
            if (lastMessage.MessageId == nextMessage.MessageId ||
                nextMessage.MessageId == initialMessage?.MessageId) return;

            ChatManager.ReceivedMessage(nextMessage);
        }

        public void ServerSendMessage(ChatMessage message)
        {
            if (!IsServer) return;
            switch (message.Channel)
            {
                case "Say" when player == null:
                {
                    // execute on the sender player's channel so we can use the observer
                    // so visible players can receive the message
                    if (PlayerManager.Instance.PlayersByName.ContainsKey(message.SenderCharacterName))
                        PlayerManager.Instance.PlayersByName[message.SenderCharacterName].Chat.Channel
                            .ServerSendMessage(message);
                    return;
                }
                case "Whisper":
                    if (player != null && message.ToCharacterName == player.Character.name)
                        LastPrivateMessage = message;
                    break;
                default:
                    LastMessage = message;
                    break;
            }
        }
    }
}