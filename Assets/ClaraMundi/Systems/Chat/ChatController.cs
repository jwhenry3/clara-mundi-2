using FishNet.Object;
using UnityEngine;

namespace ClaraMundi
{
  public class ChatController : PlayerController
  {
    public ChatChannel Channel;
    protected override void Awake()
    {
      base.Awake();
      Channel = GetComponent<ChatChannel>();
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
      SendMessageFromClient(channel, message);
    }

    [ServerRpc]
    private void SendMessageFromClient(string channel, ChatMessage message)
    {
      message.SenderCharacterName = player.Character.name;
      message.SenderPosition = player.transform.position;
      ServerSendMessage(channel, message);
    }

    public void ServerSendMessage(string channel, ChatMessage message)
    {
      if (!IsServerStarted) return;
      if (player == null) return;

      if (channel == "Party")
      {
        PartyManager.Instance.SendChatMessage(message);
        return;
      }

      if (channel != "Say" && channel != "Shout")
      {
        if (channel == "Whisper")
        {
          Channel.ServerSendMessage(message);
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
        // Debug.Log("Found sceneChat object");
        var sceneChatChannel = obj.GetComponent<ChatChannel>();
        // Debug.Log("Scene Chat Channel " + sceneChatChannel.ToString());
        if (sceneChatChannel == null) continue;
        sceneChatChannel.ServerSendMessage(message);
      }
    }
  }
}