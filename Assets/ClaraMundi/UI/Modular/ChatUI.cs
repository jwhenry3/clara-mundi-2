using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ClaraMundi
{
  public struct ParsedMessage
  {
    public string slashCommand;
    public string command;
    public string recipient;
    public List<string> arguments;
    public string messageText;
  }
  public class ChatUI : FormUI
  {
    public ChatUI Instance;
    public WindowUI window;
    public InputUI inputField;

    public ChatMessageUI ChatMessagePrefab;
    public Transform ChatMessageContainer;

    public string defaultChannel = "Say";

    public Dictionary<string, string[]> commandMap = new()
    {
      {"Tell", new[] {"/t", "/tell"}},
      {"Say", new[] {"/s", "/say"}},
      {"Shout", new[] {"/sh", "/shout"}},
      {"Yell", new[] {"/y", "/yell"}},
      {"Party", new[] {"/p", "/party"}}
    };
    public List<string> channelCommands = new() {
      "Tell", "Say", "Shout", "Yell", "Party"
    };

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
    string ParseCommand(string command)
    {
      foreach (var kvp in commandMap)
      {
        if (kvp.Value.Contains(command))
          return kvp.Key;
      }
      return "Unknown";
    }
    ParsedMessage ParseMessage(string message)
    {
      ParsedMessage parsed = new();
      List<string> words = message.Split(" ").ToList();
      parsed.recipient = null;
      if (words[0].IndexOf("/") == 0)
      {
        parsed.slashCommand = words[0];
        parsed.command = ParseCommand(words[0]);
        words.RemoveAt(0);
        if (parsed.command == "Tell")
        {
          if (words.Count > 1)
          {
            parsed.recipient = words[0];
            words.RemoveAt(0);
          }
        }
      }
      parsed.arguments = words;
      parsed.messageText = string.Join(" ", words);
      return parsed;
    }
    public override void Submit()
    {
      if (inputField.inputField.text.Length == 0) return;
      string message = inputField.inputField.text.Trim();
      int indexOfSlash = message.IndexOf("/");

      inputField.inputField.text = "";
      inputField.Select();

      if (indexOfSlash == 0)
      {
        var parsed = ParseMessage(message);
        if (parsed.command == "Tell")
        {
          if (string.IsNullOrEmpty(parsed.recipient)) return;
          ChatManager.SendChatMessage("Whisper", new ChatMessage
          {
            Type = ChatMessageType.Chat,
            Channel = "Whisper",
            Message = parsed.messageText,
            ToCharacterName = parsed.recipient
          });
          return;
        }
        if (channelCommands.Contains(parsed.command))
        {
          defaultChannel = parsed.command;
          ChatManager.SendChatMessage(defaultChannel, new ChatMessage
          {
            Type = ChatMessageType.Chat,
            Channel = defaultChannel,
            Message = parsed.messageText,
          });
          return;
        }
        else
        {
          // run a command parser
          ActionController.Instance.TriggerCommand(
            parsed.slashCommand,
            parsed.arguments.Count > 0 ? parsed.arguments[0] : "",
            parsed.arguments.Count > 1 ? parsed.arguments[1] : ""
          );
          return;
        }
      }
      ChatManager.SendChatMessage(defaultChannel, new ChatMessage
      {
        Type = ChatMessageType.Chat,
        Channel = defaultChannel,
        Message = message,
      });
      // Debug.Log("Message Sent!");
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

      Debug.Log("Message Received");
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