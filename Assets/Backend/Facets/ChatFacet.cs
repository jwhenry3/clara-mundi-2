using System.Linq;
using Unisave.Broadcasting;
using Unisave.Facades;
using Unisave.Facets;

namespace Backend.App
{
    public class ChatFacet : Facet
    {
        private CharacterEntity GetCharacter(string characterName)
        {
            var account = Auth.GetPlayer<AccountEntity>();
            return account != null ? CharacterFacet.GetByNameAndAccount(characterName, account) : null;
        }
        
        
        public ChannelSubscription SubscribeToPrivateChannel(string characterName)
        {
            var character = GetCharacter(characterName);
            if (character == null) return null;
            return CreatePrivateChannelSubscription(character.Name);
        }

        public ChannelSubscription SubscribeToGlobalChannel(string characterName, string channel)
        {
            var character = GetCharacter(characterName);
            if (character == null) return null;
            if (string.IsNullOrEmpty(channel)) return null;
            return CreateGlobalChannelSubscription(channel);
        }

        public void SendPrivateMessageTo(ChatMessage message)
        {
            var character = GetCharacter(message.senderName);
            if (character == null) return;
            message.messageId = StringUtils.UniqueId();
            Broadcast.Channel<PrivateMessageChannel>()
                .WithParameters(message.toName)
                .Send(message);
        }

        public void SendMessageToChannel(string channel, ChatMessage message)
        {
            var channels = new string[] {"Yell", "Trade", "LFG", "System"};
            if (!channels.Contains(channel)) return;
            var character = GetCharacter(message.senderName);
            if (character == null) return;
            message.messageId = StringUtils.UniqueId();
            message.senderArea = character.Area;
            Broadcast.Channel<GlobalChannel>()
                .WithParameters(channel)
                .Send(message);
        }

        public bool SendMessageToParty(ChatMessage message)
        {
            var character = GetCharacter(message.senderName);
            if (character == null) return false;
            var party = PartyFacet.GetJoinedParty(character.Name);
            if (party == null) return false;
            message.messageId = StringUtils.UniqueId();
            Broadcast.Channel<PartyChannel>()
                .WithParameters(party.PartyId)
                .Send(message);
            return true;
        }
        
        public static ChannelSubscription CreatePartyChannelSubscription(string partyId)
        {
            return Broadcast.Channel<PartyChannel>()
                .WithParameters(partyId)
                .CreateSubscription();
        }

        public static ChannelSubscription CreatePrivateChannelSubscription(string characterName)
        {
            return Broadcast.Channel<PrivateMessageChannel>()
                .WithParameters(characterName)
                .CreateSubscription();
        }
        public static ChannelSubscription CreateGlobalChannelSubscription(string channelName)
        {
            return Broadcast.Channel<GlobalChannel>()
                .WithParameters(channelName)
                .CreateSubscription();
        }
    }
}