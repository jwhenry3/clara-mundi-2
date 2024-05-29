using System;
using UnityEngine;
using System.Collections.Generic;

namespace ClaraMundi
{
  public class ChatManager : MonoBehaviour
  {
    public static event Action<ChatMessage> Messages;
    public readonly Dictionary<string, ChatChannel> Channels = new();
    public static ChatManager Instance;
    public ChatConfiguration ChatConfiguration;

    private void Awake()
    {
      if (Instance != null)
        Destroy(gameObject);
      else
        Instance = this;
    }

    public static void SendChatMessage(string channel, ChatMessage message)
    {
      if (PlayerManager.Instance.LocalPlayer)
        PlayerManager.Instance.LocalPlayer.Chat.SendMessage(channel, message);
    }
    public static void SendChatMessageFromServer(string channel, ChatMessage message)
    {
      if (PlayerManager.Instance.LocalPlayer)
        PlayerManager.Instance.LocalPlayer.Chat.SendMessage(channel, message);
    }

    public static void ReceivedMessage(ChatMessage message)
    {
      Messages?.Invoke(message);
    }

    public void OnAction(ActionInvocation action)
    {
      if (action.Action.Name == "Tell Channel")
        action.player.Chat.ServerSendMessage("Whisper", new ChatMessage
        {
          SenderCharacterName = action.player.Character.name,
          SenderPosition = action.player.transform.position,
          Type = ChatMessageType.Chat,
          Channel = "Whisper",
          Message = action.Text,
          ToCharacterName = action.Args["recipient"]
        });

      if (action.Action.Name == "Yell Channel")
        action.player.Chat.ServerSendMessage("Yell", new ChatMessage
        {
          SenderCharacterName = action.player.Character.name,
          SenderPosition = action.player.transform.position,
          Type = ChatMessageType.Chat,
          Channel = "Yell",
          Message = action.Text,
        });
      if (action.Action.Name == "Shout Channel")
        action.player.Chat.ServerSendMessage("Shout", new ChatMessage
        {
          SenderCharacterName = action.player.Character.name,
          SenderPosition = action.player.transform.position,
          Type = ChatMessageType.Chat,
          Channel = "Shout",
          Message = action.Text,
        });
      if (action.Action.Name == "Say Channel")
        action.player.Chat.ServerSendMessage("Say", new ChatMessage
        {
          SenderCharacterName = action.player.Character.name,
          SenderPosition = action.player.transform.position,
          Type = ChatMessageType.Chat,
          Channel = "Say",
          Message = action.Text,
        });
      if (action.Action.Name == "Party Channel")
        action.player.Chat.ServerSendMessage("Party", new ChatMessage
        {
          SenderCharacterName = action.player.Character.name,
          SenderPosition = action.player.transform.position,
          Type = ChatMessageType.Chat,
          Channel = "Party",
          Message = action.Text,
        });
    }
  }
}