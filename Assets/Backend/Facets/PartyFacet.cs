using System.Collections.Generic;
using Unisave.Broadcasting;
using Unisave.Facades;
using Unisave.Facets;
using Unisave.Serialization;

namespace Backend.App
{
    public class PartyFacet : Facet
    {
        private readonly int PartyMemberMax = 8;

        
        private CharacterEntity GetCharacter(string characterId)
        {
            var account = Auth.GetPlayer<AccountEntity>();
            return account != null ? CharacterFacet.GetByIdAndAccount(characterId, account) : null;
        }
        private void GetModels(string characterId, string leaderId, out CharacterEntity character,
            out CharacterEntity leader)
        {
            character = null;
            leader = null;
            var account = Auth.GetPlayer<AccountEntity>();
            if (account != null)
                character = CharacterFacet.GetByIdAndAccount(characterId, account);
            leader = CharacterFacet.GetById(leaderId);
        }
        
        private void RemovePending(CharacterEntity character)
        {
            if (character == null) return;
            var members = GetPendingByCharacter(character);
            
            foreach (var m in members)
            {
                if (m.IsInvited)
                    SendMessageTo(character, m, PartyMessageType.PlayerDeclinedInvite);
                if (m.IsRequested)
                    SendMessageTo(character, m, PartyMessageType.PlayerCancelledRequest);
                m.Delete();
            }
        }

        public ChannelSubscription SubscribeToParty(string characterId, string leaderId)
        {
            var character = GetCharacter(characterId);
            if (character == null) return null;
            var leader = GetCharacter(leaderId);
            if (leader == null) return null;
            var member = GetByCharacterAndLeader(character, leader);
            if (member == null || !member.HasJoined) return null;
            return ChatFacet.CreatePartyChannelSubscription(member.PartyId);
        }

        public void DeclineInvite(string characterId, string leaderId)
        {
            GetModels(characterId, leaderId, out var character, out var leaderCharacter);
            if (character == null) return;
            if (leaderCharacter == null) return;
            var record = GetByCharacterAndLeader(character, leaderCharacter);
            if (record == null) return;
            SendMessageTo(character, record, PartyMessageType.PlayerDeclinedInvite);
            record.Delete();
        }

        public bool InviteToParty(string characterId, string invitedCharacterId)
        {
            var character = GetCharacter(characterId);
            if (character == null) return false;
            var invitedCharacter = CharacterFacet.GetById(invitedCharacterId);
            if (invitedCharacter == null) return false;
            
            var existing = GetByCharacterAndLeader(character, character);
            var existingOther = GetByCharacter(invitedCharacter, true);
            if (existingOther != null)
            {
                return false;
            }
            
            if (existing == null)
            {
                RemovePending(character);
                existing = new PartyMemberEntity()
                {
                    PartyId = StringUtils.UniqueId(),
                    Leader = character,
                    Character = character,
                    HasJoined = true
                };
                existing.Save();
                SendPrivateMessageTo(character, PartyMessageType.PartyCreated);
            } 
            var requestedJoin = GetByCharacterAndLeader(invitedCharacter, character);
            if (requestedJoin.IsRequested)
            {
                RemovePending(character);
                requestedJoin.HasJoined = true;
                requestedJoin.IsInvited = false;
                requestedJoin.IsRequested = false;
                requestedJoin.Save();
                SendPrivateMessageTo(character, PartyMessageType.JoinedParty);
                SendMessageTo(character, requestedJoin, PartyMessageType.PlayerJoined);
                return true;
            }

            var other = new PartyMemberEntity()
            {
                PartyId = existing.PartyId,
                Character = invitedCharacter,
                Leader = character,
                IsInvited = true
            };
            other.Save();
            SendPrivateMessageTo(invitedCharacter, PartyMessageType.InvitedToParty, characterId, character.Name);
            SendPrivateMessageTo(character, PartyMessageType.PlayerInvited, invitedCharacterId, invitedCharacter.Name);

            return true;
        }

        public bool JoinParty(string characterId, string leaderId)
        {
            GetModels(characterId, leaderId, out var character, out var leaderCharacter);
            if (character == null) return false;
            if (leaderCharacter == null) return false;
            var members = GetMembersByLeader(leaderCharacter);
            if (members.Count >= PartyMemberMax)
            {
                SendPrivateMessageTo(character, PartyMessageType.PartyFull);
                return false;
            }
            var member = GetByCharacterAndLeader(character, leaderCharacter);
            // rather than request, we will just join the party if invited
            if (member != null && member.IsInvited)
            {
                RemovePending(character);
                member.HasJoined = true;
                member.IsInvited = false;
                member.IsRequested = false;
                member.Save();
                SendPrivateMessageTo(character, PartyMessageType.JoinedParty);
                SendMessageTo(character, member, PartyMessageType.PlayerJoined);
                return true;
            }
            var leaderMember = GetByCharacterAndLeader(leaderCharacter, leaderCharacter);

            if (member == null)
                member = new PartyMemberEntity()
                {
                    PartyId = leaderMember.PartyId,
                    Character = character,
                    Leader = leaderCharacter
                };
            member.IsRequested = true;
            member.Save();
            SendPrivateMessageTo(leaderCharacter, PartyMessageType.PlayerRequestedInvite, characterId, character.Name);
            SendPrivateMessageTo(character, PartyMessageType.RequestedInvite, characterId, character.Name);
            return true;
        }

        public bool LeaveParty(string characterId)
        {
            var character = GetCharacter(characterId);
            if (character == null) return false;
            var member = GetJoinedParty(character);
            if (member == null) return false;
            SendMessageTo(character, member, PartyMessageType.PlayerLeft);
            member.Delete();
            if (member.Leader != member.Character) return true;
            var party = GetMembersByLeader(character);
            PartyMemberEntity firstNonLeader = null;
            foreach (var m in party)
            {
                if (m.Character == character) continue;
                firstNonLeader = m;
                break;
            }

            if (firstNonLeader == null) return true;
            foreach (var m in party)
            {
                m.Leader = firstNonLeader.Character;
                m.Save();
            }

            var nextLeader = CharacterFacet.GetByReference(firstNonLeader.Character);
            SendMessageTo(nextLeader, member, PartyMessageType.LeaderChanged);
            return true;
        }

        public List<PartyMemberEntity> GetParty(string characterId)
        {
            var character = GetCharacter(characterId);
            if (character == null) return new List<PartyMemberEntity>();
            var member = GetJoinedParty(character);
            if (member == null) return new List<PartyMemberEntity>();
            var leader = CharacterFacet.GetByReference(member.Leader);
            return leader == null ? new List<PartyMemberEntity>() : GetMembersByLeader(leader);
        }

        public static List<PartyMemberEntity> GetMembersByLeader(CharacterEntity character)
        {
            return DB.TakeAll<PartyMemberEntity>().Filter((p) => p.Leader == character && p.HasJoined).Get();
        }

        public static PartyMemberEntity GetLeader(CharacterEntity character)
        {
            return DB.TakeAll<PartyMemberEntity>().Filter((p) => p.Leader == character && p.Character == character)
                .First();
        }

        public static PartyMemberEntity GetByCharacterAndLeader(CharacterEntity character, CharacterEntity leader)
        {
            return DB.TakeAll<PartyMemberEntity>().Filter((p) => p.Character == character && p.Leader == leader)
                .First();
        }
        public static PartyMemberEntity GetByCharacter(CharacterEntity character, bool hasJoined)
        {
            return DB.TakeAll<PartyMemberEntity>().Filter((p) => p.Character == character && p.HasJoined == hasJoined)
                .First();
        }

        public static PartyMemberEntity GetJoinedParty(CharacterEntity character)
        {
            return DB.TakeAll<PartyMemberEntity>().Filter((p) => p.Character == character && p.HasJoined).First();
        }

        public static List<PartyMemberEntity> GetPendingByCharacter(CharacterEntity character)
        {
            return DB.TakeAll<PartyMemberEntity>().Filter((p) => p.Character == character && !p.HasJoined).Get();
        }

        public static void SendMessageTo(CharacterEntity character, PartyMemberEntity member, PartyMessageType type)
        {
            Broadcast.Channel<PartyChannel>()
                .WithParameters(member.PartyId)
                .Send(new PartyMessage()
                {
                    type = type,
                    characterId = character.EntityId,
                    characterName = character.Name
                });
        }

        public static void SendPrivateMessageTo(CharacterEntity character, PartyMessageType type, string characterId = null, string characterName = null)
        {
            Broadcast.Channel<PrivateMessageChannel>()
                .WithParameters(character.Name)
                .Send(new PartyMessage()
                {
                    type = type,
                    characterId = characterId,
                    characterName = characterName
                });
        }

    }
}