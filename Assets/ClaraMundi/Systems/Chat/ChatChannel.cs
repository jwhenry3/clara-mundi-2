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
        [SyncVar(OnChange = nameof(OnMessage))]
        public ChatMessage LastMessage;

        private ChatMessage initialMessage;
        public override void OnStartServer()
        {
            base.OnStartServer();
            ChatManager.Instance.Channels[gameObject.scene.name] = this;
        }
        public override void OnStartClient()
        {
            base.OnStartClient();
            initialMessage = LastMessage;
            ChatManager.ReceivedMessage(new ChatMessage
            {
                Type = ChatMessageType.System,
                Message = $"Joined the Say Channel"
            });
            ChatManager.ReceivedMessage(new ChatMessage
            {
                Type = ChatMessageType.System,
                Message = $"Joined the Shout Channel"
            });
        }

        private void OnDestroy()
        {
            if (ChatManager.Instance.Channels.ContainsKey(gameObject.scene.name))
                ChatManager.Instance.Channels[gameObject.scene.name] = null;
            ChatManager.ReceivedMessage(new ChatMessage
            {
                Type = ChatMessageType.System,
                Message = $"Left the Say Channel"
            });
            ChatManager.ReceivedMessage(new ChatMessage
            {
                Type = ChatMessageType.System,
                Message = $"Left the Shout Channel"
            });
        }

        private void OnMessage(ChatMessage lastMessage, ChatMessage nextMessage, bool asServer)
        {
            if (asServer) return;
            if (nextMessage.Channel != "Say" && nextMessage.Channel != "Shout") return;
            if (lastMessage.MessageId == nextMessage.MessageId || nextMessage.MessageId == initialMessage?.MessageId) return;
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