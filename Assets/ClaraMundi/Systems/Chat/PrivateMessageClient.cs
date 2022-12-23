using System;
using Backend.App;
using Unisave.Broadcasting;
using Unisave.Facades;

public class PrivateMessageClient : UnisaveBroadcastingClient
{
    public ChannelSubscription Subscription;
    private void OnEnable()
    {
    }

    public async void SubscribeTo(string partyId)
    {
        Subscription = await OnFacet<PartyFacet>.CallAsync<ChannelSubscription>(
            nameof(PartyFacet.SubscribeToParty),
            partyId
        );
        FromSubscription(Subscription)
            .Forward<PartyMessage>(PartyMessageReceived)
            .ElseLogWarning();
    }

    void PartyMessageReceived(PartyMessage message)
    {
        // do something
    }
}