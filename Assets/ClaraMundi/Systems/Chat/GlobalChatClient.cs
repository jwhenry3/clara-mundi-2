using System;
using System.Collections.Generic;
using Backend.App;
using ClaraMundi;
using Unisave.Broadcasting;
using Unisave.Facades;
using UnityEngine;

public class GlobalChatClient : UnisaveBroadcastingClient
{
    public Dictionary<string, ChannelSubscription> Subscriptions = new();

    private Player player;

    private bool started;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        player.NetStarted += OnNetStarted;
        if (player.Entity.Character != null && !string.IsNullOrEmpty(player.Entity.Character.Name))
            OnNetStarted();
    }

    private void OnNetStarted()
    {
        if (started) return;
        started = true;
        if (!player.Entity.IsOwner) return;
        SubscribeTo("System");
        SubscribeTo("LFG");
        SubscribeTo("Trade");
        SubscribeTo("Yell");
    }

    private void OnEnable()
    {
        if (player.Entity.Character != null && !string.IsNullOrEmpty(player.Entity.Character.Name))
            OnNetStarted();
    }

    public async void SubscribeTo(string channel)
    {
        if (!player.Entity.IsOwner) return;
        if (Subscriptions.ContainsKey(channel))
        {
        }

        Subscriptions[channel] = await OnFacet<ChatFacet>.CallAsync<ChannelSubscription>(
            nameof(ChatFacet.SubscribeToGlobalChannel),
            player.Character.Name,
            channel
        );
        if (Subscriptions[channel] == null)
        {
            Debug.LogWarning($"Could not subscribe to the {channel} channel!");
            return;
        }

        FromSubscription(Subscriptions[channel])
            .Forward<Backend.ChatMessage>(OnChannelMessage(channel))
            .ElseLogWarning();
        ChatManager.ReceivedMessage(new ChatMessage()
        {
            Type = ChatMessageType.System,
            Channel = "System",
            Message = $"Joined the {channel} Channel"
        });
    }

    Action<Backend.ChatMessage> OnChannelMessage(string channel) =>
        (Backend.ChatMessage message) => ChatMessageReceived(channel, message);

    void ChatMessageReceived(string channel, Backend.ChatMessage message)
    {
        ChatManager.ReceivedMessage(new ChatMessage()
        {
            Type = ChatMessageType.Chat,
            Channel = channel,
            Message = message.message,
            MessageId = message.messageId,
            SenderArea =  message.senderArea,
            SenderCharacterName = message.senderName,
            ToCharacterName = message.toName,
        });
    }

    public void SendChatMessage(string channel, ChatMessage message)
    {
        OnFacet<ChatFacet>.CallAsync(
            nameof(ChatFacet.SendMessageToChannel),
            channel,
            new Backend.ChatMessage()
            {
                senderName = message.SenderCharacterName,
                toName = message.ToCharacterName,
                message = message.Message
            });
    }
}