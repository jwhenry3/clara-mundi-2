using UnityEngine;
using FishNet;
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
        Player Player;
        [SyncVar]
        public string PlayerName;
        [SyncVar]
        public string Name;

        public ChannelScope ChannelScope = ChannelScope.Scene;
        public ChannelType ChannelType = ChannelType.System;


        [SyncVar(OnChange = "OnMessage")]
        public ChatMessage LastPublicMessage;

        [SyncVar(OnChange = "OnMessage", ReadPermissions = ReadPermission.OwnerOnly)]
        public ChatMessage LastPrivateMessage;
        bool initialized;
        public override void OnStartServer()
        {
            base.OnStartServer();
            Player = GetComponentInParent<Player>();
            if (Player)
                PlayerName = Player.Entity.entityName;
            if (!string.IsNullOrEmpty(PlayerName))
                Name = "player:" + PlayerName;
            ChatManager.Instance.Channels[Name] = this;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            Player = GetComponentInParent<Player>();
            ChatManager.Instance.Channels[Name] = this;
            if (Name.Contains("player:")) return;
            ChatManager.ReceivedMessage(new ChatMessage
            {
                Type = ChatMessageType.System,
                Message = $"Joined the {Name} Channel"
            });
        }

        private void OnDestroy()
        {
            if (ChatManager.Instance.Channels.ContainsKey(Name))
                ChatManager.Instance.Channels.Remove(Name);
            if (Name.Contains("player:")) return;
            ChatManager.ReceivedMessage(new ChatMessage
            {
                Type = ChatMessageType.System,
                Message = $"Left the {Name} Channel"
            });
        }

        private void OnMessage(ChatMessage lastMessage, ChatMessage nextMessage, bool asServer)
        {
            if (asServer) return;
            nextMessage.ChannelType = ChannelType;
            if (ChannelScope == ChannelScope.Local)
            {
                if (PlayerManager.Instance.LocalPlayer == null) return;
                if (Vector3.Distance(PlayerManager.Instance.LocalPlayer.transform.position, nextMessage.SenderPosition) < 50)
                    ChatManager.ReceivedMessage(nextMessage);
                return;
            }
            if (ChannelScope == ChannelScope.Private && (bool)Player) {
                nextMessage.ToEntityId = Player.Entity.entityId;
            }
            ChatManager.ReceivedMessage(nextMessage);
        }

        public void ServerSendMessage(ChatMessage message)
        {
            if (!IsServer) return;
            if (ChannelScope == ChannelScope.Private)
                LastPrivateMessage = message;
            else
                LastPublicMessage = message;
        }

    }
}