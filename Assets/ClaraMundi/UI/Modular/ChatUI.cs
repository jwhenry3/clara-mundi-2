using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ClaraMundi
{
  public class ChatUI : FormUI
  {
    public ChatUI Instance;
    public WindowUI window;
    public InputUI inputField;

    public ChatMessageUI ChatMessagePrefab;
    public Transform ChatMessageContainer;

    public string defaultChannel = "Say";

    private void Start()
    {
      Instance = this;
      ChatManager.Messages += OnMessage;
      ClearMessages();
      // UpdateAttachmentList();

    }

    private void OnDestroy()
    {
      ChatManager.Messages -= OnMessage;
    }
    public override void Submit()
    {
      if (inputField.inputField.text.Length == 0) return;
      string message = inputField.inputField.text;
      inputField.inputField.text = "";
      inputField.Select();
      if (message.IndexOf("/t ") == 0 || message.IndexOf("/tell ") == 0)
      {
        message = message.Substring("/t ".Length, message.Length - "/t ".Length);
        int indexOfSpace = message.IndexOf(" ");
        if (indexOfSpace > -1)
        {
          string recipient = message.Substring(0, indexOfSpace);
          message = message.Substring(indexOfSpace + 1, message.Length - indexOfSpace - 1);
          ChatManager.SendChatMessage("Whisper", new ChatMessage
          {
            Type = ChatMessageType.Chat,
            Channel = "Whisper",
            Message = message,
            ToCharacterName = recipient
          });
        }
        return;
      }
      if (message.IndexOf("/s ") == 0 || message.IndexOf("/say ") == 0)
      {
        int indexOfSpace = message.IndexOf(" ");
        defaultChannel = "Say";
        message = message.Substring(indexOfSpace + 1, message.Length - indexOfSpace - 1);
        ChatManager.SendChatMessage(defaultChannel, new ChatMessage
        {
          Type = ChatMessageType.Chat,
          Channel = "Say",
          Message = message,
        });
        return;
      }
      if (message.IndexOf("/sh ") == 0 || message.IndexOf("/shout ") == 0)
      {
        int indexOfSpace = message.IndexOf(" ");
        defaultChannel = "Shout";
        message = message.Substring(indexOfSpace + 1, message.Length - indexOfSpace - 1);
        ChatManager.SendChatMessage(defaultChannel, new ChatMessage
        {
          Type = ChatMessageType.Chat,
          Channel = "Shout",
          Message = message,
        });
        return;
      }
      if (message.IndexOf("/y ") == 0 || message.IndexOf("/yell ") == 0)
      {
        int indexOfSpace = message.IndexOf(" ");
        defaultChannel = "Yell";
        message = message.Substring(indexOfSpace + 1, message.Length - indexOfSpace - 1);
        ChatManager.SendChatMessage(defaultChannel, new ChatMessage
        {
          Type = ChatMessageType.Chat,
          Channel = "Yell",
          Message = message,
        });
        return;
      }
      if (message.IndexOf("/p ") == 0 || message.IndexOf("/party ") == 0)
      {
        int indexOfSpace = message.IndexOf(" ");
        defaultChannel = "Party";
        message = message.Substring(indexOfSpace + 1, message.Length - indexOfSpace - 1);
        ChatManager.SendChatMessage(defaultChannel, new ChatMessage
        {
          Type = ChatMessageType.Chat,
          Channel = "Party",
          Message = message,
        });
        return;
      }
      ChatManager.SendChatMessage(defaultChannel, new ChatMessage
      {
        Type = ChatMessageType.Chat,
        Channel = defaultChannel,
        Message = message,
      });
    }

    public void ClearMessages()
    {
      if (ChatMessageContainer == null) return;
      foreach (Transform child in ChatMessageContainer)
        Destroy(child.gameObject);
    }

    private void OnMessage(ChatMessage message)
    {
      if (ChatMessageContainer == null) return;
      ClearOutOfBounds();

      var instance = Instantiate(ChatMessagePrefab);
      instance.SetChatMessage(message);
      instance.transform.SetParent(ChatMessageContainer);
      if (ChatMessageContainer.childCount > 100)
        Destroy(ChatMessageContainer.GetChild(0).gameObject);
    }

    void ClearOutOfBounds()
    {
      if (ChatMessageContainer == null) return;
      if (ChatMessageContainer.childCount <= 99) return;
      // remove the oldest first
      Destroy(ChatMessageContainer.GetChild(0).gameObject);
      // recurse until no longer out of bounds
      ClearOutOfBounds();
    }
  }
}