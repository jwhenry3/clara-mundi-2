using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
  public class ChatMessageUI : MonoBehaviour, IPointerMoveHandler, IPointerDownHandler
  {
    public string NodeId = StringUtils.UniqueId();
    TextMeshProUGUI Text;
    public ItemTooltipUI ItemTooltip => ChatWindowUI.Instance.ItemTooltip;
    public ChatMessage ChatMessage;


    string sender;

    string color;

    private void Awake()
    {
      Text = GetComponent<TextMeshProUGUI>();
    }

    private void OnDestroy()
    {
      if (ItemTooltip.NodeId == NodeId)
        ItemTooltip.gameObject.SetActive(false);
    }

    public void SetChatMessage(ChatMessage message)
    {
      ChatMessage = message;
      UpdateColor();
      UpdateSenderStyle();
      SaveToText();
    }

    private void UpdateColor()
    {
      color = ChatManager.Instance.ChatConfiguration.GetColor(ChatMessage.Type, ChatMessage.Channel);
    }

    private void UpdateSenderStyle()
    {
      if (ChatMessage.Type == ChatMessageType.System)
      {
        sender = "";
        return;
      }

      var senderName = "System";
      var receiverName = "";
      if (!string.IsNullOrEmpty(ChatMessage.SenderCharacterName))
        senderName = Player.GetClickableName(ChatMessage.SenderCharacterName);
      if (!string.IsNullOrEmpty(ChatMessage.ToCharacterName))
        receiverName = Player.GetClickableName(ChatMessage.ToCharacterName);

      if (ChatMessage.Channel == "Whisper" &&
          ChatMessage.SenderCharacterName == null)
        sender = "[Whisper To] " + receiverName + ":";
      else if (ChatMessage.Channel == "Whisper")
        sender = "[Whisper From] " + senderName + ":";
      else if (ChatMessage.Channel == "Yell" && !string.IsNullOrEmpty(ChatMessage.SenderArea))
        sender = "[" + ChatMessage.Channel + "][" + ChatMessage.SenderArea + "] " + senderName + ":";
      else
        sender = "[" + ChatMessage.Channel + "] " + senderName + ":";
    }

    void SaveToText()
    {
      if (sender == "")
        Text.text = $"<color={color}>{ChatMessage.Message}</color>";
      else
        Text.text = $"<color={color}>{sender} {ChatMessage.Message}</color>";
    }

    public void OnPointerMove(PointerEventData eventData)
    {
      var selectedLink = TextUtils.GetLinkUnder(Text, eventData);
      if (selectedLink.Contains("item:"))
      {
        if (ItemTooltip.NodeId == NodeId) return;
        ItemTooltip.NodeId = NodeId;
        ShowTooltip(selectedLink["item:".Length..]);
        return;
      }

      ItemTooltip.NodeId = null;
      ItemTooltip.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
      var selectedLink = TextUtils.GetLinkUnder(Text, eventData);
      if (selectedLink == "") return;
      if (!selectedLink.Contains("player:")) return;
      if (eventData.button == PointerEventData.InputButton.Right)
        ChatWindowUI.Instance.OpenPlayerContextMenu(eventData, selectedLink["player:".Length..]);
    }

    private void ShowTooltip(string itemOrInstanceId)
    {
      ItemTooltipUtils.ShowTooltip(ItemTooltip, (RectTransform)transform, itemOrInstanceId);
    }
  }
}