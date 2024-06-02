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

    public void Init()
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
      string message = inputField.inputField.text.Trim();
      int indexOfSlash = message.IndexOf("/");

      inputField.inputField.text = "";
      inputField.Select();

      if (indexOfSlash == 0)
      {
        var words = message.Split(" ").ToList();
        var command = words[0];
        words.RemoveAt(0);
        var text = string.Join(" ", words);
        if (command == "/channel" || command == "/chan" || command == "/cmd")
        {
          if (words.Count > 0)
            defaultChannel = words[0];
          return;
        }
        // run a command parser
        ActionController.Instance.TriggerCommand(
          command,
          text
        );
        return;
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

      // Debug.Log("Message Received");
      // Debug.Log(message);
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

    void Update()
    {
      if (!inputField.gameObject.activeInHierarchy && Input.GetKey("/"))
      {
        inputField.Init();
        window.moveSibling.ToFront();
        inputField.inputField.text = "/";
        inputField.inputField.caretPosition = 1;
      }
    }
  }
}