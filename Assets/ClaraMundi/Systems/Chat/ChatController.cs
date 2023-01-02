using FishNet.Object;
using UnityEngine;

namespace ClaraMundi
{
    public class ChatController : PlayerController
    {
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

            message.SenderCharacterName = player.Character.name;
            message.SenderPosition = player.transform.position;
            switch (channel)
            {
                case "Party":
                    player.Party.SendChatMessage(message);
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
            message.SenderCharacterName = player.Character.name;
            message.SenderPosition = player.transform.position;
            ServerSendMessage(channel, message);
        }

        private void ServerSendMessage(string channel, ChatMessage message)
        {
            if (!IsServer) return;
            if (player == null) return;
            if (channel != "Say" && channel != "Shout")
            {
                if (channel == "Whisper")
                {
                    if (!ChatManager.Instance.Channels.ContainsKey(message.ToCharacterName)) return;
                    ChatManager.Instance.Channels[message.ToCharacterName].ServerSendMessage(message);
                    return;
                }

                if (!ChatManager.Instance.Channels.ContainsKey(channel)) return;
                ChatManager.Instance.Channels[channel].ServerSendMessage(message);
                return;
            }

            var found = GameObject.FindGameObjectsWithTag("SceneChat");
            foreach (var obj in found)
            {
                if (obj.scene.handle != player.gameObject.scene.handle || obj.scene.name != player.gameObject.scene.name) continue;
                Debug.Log("Found sceneChat object");
                var sceneChatChannel = obj.GetComponent<ChatChannel>();
                if (sceneChatChannel == null) continue;
                sceneChatChannel.ServerSendMessage(message);
            }
        }
    }
}