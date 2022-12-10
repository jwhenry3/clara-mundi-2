using FishNet.Object;

namespace ClaraMundi
{
    public class ChatController : PlayerController
    {
        public void SendMessage(string channel, ChatMessage message)
        {
            if (!string.IsNullOrEmpty(message.ToEntityId) && message.ChannelType == ChannelType.Whisper)
            {
                // send the chat message to the chat window even though this is outgoing and not incoming
                // the private channel on the to-entity will not send the message to the sender, so we must
                // display it client-side only
                ChatManager.ReceivedMessage(message);
            }

            SendMessageFromClient(channel, message);
        }
        [ServerRpc]
        private void SendMessageFromClient(string channel, ChatMessage message)
        {
            if (!IsServer) return;
            message.SenderEntityId = player.entityId;
            ServerSendMessage(channel, message);
        }
        public void ServerSendMessage(string channel, ChatMessage message)
        {
            if (!IsServer) return;
            if (channel == "Party")
            {
                PartyManager.Instance.ServerSendMessage(message);
                return;
            }
            if (!ChatManager.Instance.Channels.ContainsKey(channel)) return;
            
            if (!string.IsNullOrEmpty(message.SenderEntityId))
            {
                // track position for local channels
                message.SenderPosition = EntityManager.Instance.Entities[message.SenderEntityId].transform.position;
            }
            ChatManager.Instance.Channels[channel].ServerSendMessage(message);
        }
    }
}