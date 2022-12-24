using Backend.App;
using ClaraMundi;
using Unisave.Broadcasting;
using Unisave.Facades;

public class PrivateMessageClient : UnisaveBroadcastingClient
{
    public ChannelSubscription Subscription;
    private Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    private async void OnEnable()
    {
        if (player == null || string.IsNullOrEmpty(player.Character?.Name)) return;
        // do not listen to private messages unless the owning player
        if (!player.Entity.IsOwner) return;
        Subscription = await OnFacet<ChatFacet>.CallAsync<ChannelSubscription>(
            nameof(ChatFacet.SubscribeToPrivateChannel),
            player.Character.Name
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
        }
    }

    
    void ChatMessageReceived(ChatMessage message)
    {
        ChatManager.ReceivedMessage(new ClaraMundi.ChatMessage()
        {
            Type = ChatMessageType.Chat,
            Channel = player.Character.Name,
            Message = message.message,
            MessageId = message.messageId,
            SenderCharacterName = message.senderName,
            ToCharacterName = player.Character.Name,
        });
    }
}