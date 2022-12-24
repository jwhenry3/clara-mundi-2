using System;
using Backend.App;
using ClaraMundi;
using Unisave.Broadcasting;
using Unisave.Facades;
using UnityEngine;

public class PartyClient : UnisaveBroadcastingClient
{
    public ChannelSubscription Subscription;

    private Player player;
    private string partyId;

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
        player.Party.PartyChanges += SubscribeTo;
    }

    private void OnEnable()
    {
        if (player.Entity.Character != null && !string.IsNullOrEmpty(player.Entity.Character.Name))
            OnNetStarted();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (!player.Entity.IsOwner) return;
        if (started)
        {
            started = false;
            player.Party.PartyChanges -= SubscribeTo;
        }

        ChatManager.ReceivedMessage(new ChatMessage()
        {
            Type = ChatMessageType.System,
            Channel = "System",
            Message = "Left the Party Channel"
        });
    }

    public async void SubscribeTo(PartyModel party)
    {
        Debug.Log("Subscribe to party channel 1");
        if (!player.Entity.IsOwner) return;
        if (Subscription != null && partyId != party.PartyId)
        {
            // let the parent class handle cleanup of the previous party subscription
            OnDisable();
        }
        // no need to start a subscription if the player has left a party
        if (party == null) return;
        if (party.PartyId == partyId && Subscription != null) return;
        partyId = party.PartyId;
        Debug.Log("Subscribe to party channel 2");
        Subscription = await OnFacet<PartyFacet>.CallAsync<ChannelSubscription>(
            nameof(PartyFacet.SubscribeToParty),
            player.Character.Name
        );
        if (Subscription == null)
        {
            Debug.LogWarning("Could not subscribe to the party messages!");
            return;
        }
        FromSubscription(Subscription)
            .Forward<PartyMessage>(PartyMessageReceived)
            .Forward<Backend.ChatMessage>(ChatMessageReceived)
            .ElseLogWarning();
        ChatManager.ReceivedMessage(new ChatMessage()
        {
            Type = ChatMessageType.System,
            Channel = "System",
            Message = "Joined the Party Channel"
        });
    }

    
    void PartyMessageReceived(PartyMessage message)
    {
        // do something
        switch (message.type)
        {
            case PartyMessageType.LeaderChanged:
                player.Party.GetParty();
                break;
            case PartyMessageType.PartyDisbanded:
                player.Party.ClearParty();
                break;
            case PartyMessageType.PlayerJoined:
                player.Party.MemberJoined(message);
                break;
            case PartyMessageType.PlayerLeft:
                player.Party.MemberLeft(message);
                break;
            case PartyMessageType.PartyFull:
                player.Party.PartyFull();
                break;
        }
    }

    void ChatMessageReceived(Backend.ChatMessage message)
    {
        
        ChatManager.ReceivedMessage(new ChatMessage()
        {
            Type = ChatMessageType.Chat,
            Channel = "Party",
            Message = message.message,
            MessageId = message.messageId,
            SenderCharacterName = message.senderName,
            ToCharacterName = message.toName,
        });
    }

    public static bool DoesPartyExist(PartyModel party)
    {
        return party != null && !string.IsNullOrEmpty(party.Leader) && !string.IsNullOrEmpty(party.PartyId);
    }
}