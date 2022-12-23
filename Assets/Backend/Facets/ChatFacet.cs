using System.Linq;
using Unisave.Broadcasting;
using Unisave.Facades;
using Unisave.Facets;

namespace Backend.App
{
    public class ChatFacet : Facet
    {
        private CharacterEntity GetCharacter(string characterId)
        {
            var account = Auth.GetPlayer<AccountEntity>();
            return account != null ? CharacterFacet.GetByIdAndAccount(characterId, account) : null;
        }
        private CharacterEntity GetCharacterByName(string characterName)
        {
            var account = Auth.GetPlayer<AccountEntity>();
            return account != null ? CharacterFacet.GetByNameAndAccount(characterName, account) : null;
        }
        
        
        public ChannelSubscription SubscribeToPrivateChannel(string characterId)
        {
            var character = GetCharacter(characterId);
            if (character == null) return null;
            return CreatePrivateChannelSubscription(character.Name);
        }

        public void SendPrivateMessageTo(ChatMessage message)
        {
            var character = GetCharacterByName(message.senderName);
            if (character == null) return;
            message.messageId = StringUtils.UniqueId();
            Broadcast.Channel<PrivateMessageChannel>()
                .WithParameters(message.toName)
                .Send(message);
        }

        public void SendMessageToChannel(string channel, ChatMessage message)
        {
            var channels = new string[] {"yell", "trade", "lfg"};
            if (!channels.Contains(channel)) return;
            var character = GetCharacterByName(message.senderName);
            if (character == null) return;
            message.messageId = StringUtils.UniqueId();
            Broadcast.Channel<GlobalChannel>()
                .WithParameters(channel)
                .Send(message);
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
    }
}