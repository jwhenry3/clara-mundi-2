using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

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

    public readonly SyncVar<ChatMessage> LastMessage = new();

    public readonly SyncVar<ChatMessage> LastPrivateMessage = new(new SyncTypeSettings(ReadPermission.OwnerOnly));

    private ChatMessage initialMessage;

    void OnEnable()
    {
      LastMessage.OnChange += OnMessage;
      LastPrivateMessage.OnChange += OnMessage;
    }

    void OnDisable()
    {
      LastMessage.OnChange -= OnMessage;
      LastPrivateMessage.OnChange -= OnMessage;
    }

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
      initialMessage = LastMessage.Value;
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
        if (!IsServerStarted) return;
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
        if (!IsServerStarted) continue;
        ChatManager.Instance.Channels.Remove(channel);
      }
    }

    private void OnMessage(ChatMessage lastMessage, ChatMessage nextMessage, bool asServer)
    {
      // Debug.Log("On Message");
      if (asServer) return;
      // Debug.Log("On Message: " + nextMessage.Message);
      if (initialMessage != null && initialMessage.MessageId == nextMessage.MessageId) return;
      if (lastMessage != null && lastMessage.MessageId == nextMessage.MessageId) return;
      ChatManager.ReceivedMessage(nextMessage);
    }

    public void ServerSendMessage(ChatMessage message)
    {
      if (!IsServerStarted) return;
      // Debug.Log("Server Send Message");
      // Debug.Log(message.Channel + " - " + message.Message + " - " + (player != null ? "player" : "no player") + " - " + message.SenderCharacterName);
      switch (message.Channel)
      {
        case "Say" when player == null:
          {
            if (string.IsNullOrEmpty(message.SenderCharacterName)) return;
            // execute on the sender player's channel so we can use the observer
            // so visible players can receive the message
            var player = PlayerManager.Instance.GetPlayer(message.SenderCharacterName);
            if (player != null)
              player.Chat.Channel.ServerSendMessage(message);
            return;
          }
        case "Whisper":
          if (player != null && message.ToCharacterName == player.Character.name)
            LastPrivateMessage.Value = message;
          break;
        default:
          // Debug.Log("Set Last Message");
          LastMessage.Value = message;
          break;
      }
    }
  }
}