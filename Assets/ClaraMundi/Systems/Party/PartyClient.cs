using Backend.App;
using ClaraMundi;
using Unisave.Broadcasting;
using Unisave.Facades;

public class PartyClient : UnisaveBroadcastingClient
{
    public ChannelSubscription Subscription;

    private Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    public async void SubscribeToParty(string partyId)
    {
        if (!player.Entity.IsOwner) return;
        if (Subscription != null)
        {
            // let the parent class handle cleanup of the previous party subscription
            OnDisable();
        }
        // no need to start a subscription if the player has left a party
        if (string.IsNullOrEmpty(partyId)) return;
        
        Subscription = await OnFacet<PartyFacet>.CallAsync<ChannelSubscription>(
            nameof(PartyFacet.SubscribeToParty),
            partyId
        );
        FromSubscription(Subscription)
            .Forward<PartyMessage>(PartyMessageReceived)
            .Forward<ChatMessage>(ChatMessageReceived)
            .ElseLogWarning();
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

    void ChatMessageReceived(ChatMessage message)
    {
        ChatManager.ReceivedMessage(new ClaraMundi.ChatMessage()
        {
            Type = ChatMessageType.Chat,
            Channel = "party",
            Message = message.message,
            MessageId = message.messageId,
            SenderCharacterName = message.senderName,
            ToCharacterName = message.toName,
        });
    }
}