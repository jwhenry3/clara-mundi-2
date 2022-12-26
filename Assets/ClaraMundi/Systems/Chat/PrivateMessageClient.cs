using System;
using Backend.App;
using ClaraMundi;
using Unisave.Broadcasting;
using Unisave.Facades;
using Unisave.Serialization;
using UnityEngine;

public class PrivateMessageClient : UnisaveBroadcastingClient
{
    public ChannelSubscription Subscription;
    private Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        player.NetStarted += Subscribe;
        if (player.Entity.Character != null && !string.IsNullOrEmpty(player.Entity.Character.Name))
            Subscribe();
    }

    private void OnDestroy()
    {
        if (player == null) return;
        player.NetStarted -= Subscribe;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (!player.Entity.IsOwner) return;
        player.NetStarted -= Subscribe;
        ChatManager.ReceivedMessage(new ChatMessage()
        {
            Type = ChatMessageType.System,
            Channel = "System",
            Message = "Left Private Channel"
        });
    }

    private async void Subscribe()
    {
        if (Subscription != null)
        {
            // let the parent class handle cleanup of the previous party subscription
            OnDisable();
        }

        // do not listen to private messages unless the owning player
        if (player.Entity.Character == null || string.IsNullOrEmpty(player.Entity.Character.Name)) return;
        if (!player.Entity.IsOwner) return;
        Debug.Log("SUBSCRIBE!");
        Subscription = await OnFacet<ChatFacet>.CallAsync<ChannelSubscription>(
            nameof(ChatFacet.SubscribeToPrivateChannel),
            player.Character.Name
        );
        FromSubscription(Subscription)
            .Forward<PartyMessage>(PartyMessageReceived)
            .Forward<Backend.ChatMessage>(ChatMessageReceived)
            .ElseLogWarning();
        ChatManager.ReceivedMessage(new ChatMessage()
        {
            Type = ChatMessageType.System,
            Channel = "System",
            Message = "Joined your Private Channel"
        });
    }

    public void SendChatMessage(ChatMessage message)
    {
        OnFacet<ChatFacet>.CallAsync(
            nameof(ChatFacet.SendPrivateMessageTo),
            new Backend.ChatMessage()
            {
                senderName = message.SenderCharacterName,
                toName = message.ToCharacterName,
                message = message.Message
            });
    }

    void PartyMessageReceived(PartyMessage message)
    {
        // do something
        switch (message.type)
        {
            case PartyMessageType.Private_PlayerDeclinedInvite:
                player.Party.PlayerDeclinedInvite(message);
                break;
            case PartyMessageType.Private_PlayerCancelledRequest:
                player.Party.PlayerCancelledRequest(message);
                break;
            case PartyMessageType.Private_PlayerRequestedInvite:
                player.Party.PlayerRequestedInvite(message);
                break;
            case PartyMessageType.Private_PartyCreated:
            case PartyMessageType.Private_JoinedParty:
                player.Party.GetParty();
                break;
            case PartyMessageType.Private_LeftParty:
                player.Party.ClearParty();
                break;
            case PartyMessageType.Private_InvitedToParty:
                player.Party.InvitedToParty(message);
                break;
            case PartyMessageType.Private_PlayerInvited:
                player.Party.PlayerInvited(message);
                break;
            case PartyMessageType.Private_AlreadyInParty:
                player.Party.AlreadyInParty(message);
                break;
        }
    }


    void ChatMessageReceived(Backend.ChatMessage message)
    {
        ChatManager.ReceivedMessage(new ClaraMundi.ChatMessage()
        {
            Type = ChatMessageType.Chat,
            Channel = "Whisper",
            Message = message.message,
            MessageId = message.messageId,
            SenderCharacterName = message.senderName,
            ToCharacterName = player.Character.Name,
        });
    }
}