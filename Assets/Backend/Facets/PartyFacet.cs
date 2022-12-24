using System.Collections.Generic;
using Unisave.Broadcasting;
using Unisave.Facades;
using Unisave.Facets;
using UnityEngine;

namespace Backend.App
{
    public class PartyFacet : Facet
    {
        private readonly int PartyMemberMax = 8;


        private CharacterEntity GetCharacter(string characterName)
        {
            var account = Auth.GetPlayer<AccountEntity>();
            return account != null ? CharacterFacet.GetByNameAndAccount(characterName, account) : null;
        }

        private void RemovePending(string character)
        {
            if (character == null) return;
            var members = GetPendingByCharacter(character);

            foreach (var m in members)
            {
                if (m.IsInvited)
                    SendMessageToPlayer(m.LeaderName, PartyMessageType.Private_PlayerDeclinedInvite, character);

                if (m.IsRequested)
                    SendMessageToPlayer(m.LeaderName, PartyMessageType.Private_PlayerCancelledRequest, character);

                m.Delete();
            }
        }

        public bool IsPlayerInParty(string characterName, string playerName)
        {
            if (GetCharacter(characterName) == null) return false;
            return GetJoinedParty(playerName) != null;
        }
        public ChannelSubscription SubscribeToParty(string characterName)
        {
            var character = GetCharacter(characterName);
            if (character == null) return null;
            var member = GetJoinedParty(characterName);
            if (member == null) return null;
            return ChatFacet.CreatePartyChannelSubscription(member.PartyId);
        }

        public void DeclineInvite(string characterName, string leaderName)
        {
            var character = GetCharacter(characterName);
            if (character == null) return;
            var record = GetByCharacterAndLeader(characterName, leaderName);
            if (record == null) return;
            SendMessageToPlayer(leaderName, PartyMessageType.Private_PlayerDeclinedInvite, character.Name);
            record.Delete();
        }

        public void DeclineRequest(string characterName, string requestedCharacterName)
        {
            var character = GetCharacter(characterName);
            if (character == null) return;
            var record = GetByCharacterAndLeader(requestedCharacterName, characterName);
            if (record == null) return;
            SendMessageToPlayer(requestedCharacterName, PartyMessageType.Private_RequestDenied, character.Name);
            record.Delete();
        }

        public bool CreateParty(string characterName)
        {
            var character = GetCharacter(characterName);
            if (character == null) return false;
            var existing = GetMemberByCharacter(characterName, true);
            if (existing != null)
            {
                SendMessageToPlayer(characterName, PartyMessageType.Private_AlreadyInParty);
                return false;
            }
            RemovePending(characterName);
            var member = new PartyMemberEntity()
            {
                PartyId = StringUtils.UniqueId(),
                LeaderName = characterName,
                MemberName = characterName,
                HasJoined = true
            };
            member.Save();
            SendMessageToPlayer(characterName, PartyMessageType.Private_PartyCreated, null, member.PartyId);
            return true;
        }

        public bool InviteToParty(string characterName, string invitedCharacterName)
        {
            var character = GetCharacter(characterName);
            if (character == null) return false;
            var invitedCharacter = CharacterFacet.GetByName(invitedCharacterName);
            if (invitedCharacter == null) return false;

            var existing = GetByCharacterAndLeader(characterName, characterName);
            var existingOther = GetMemberByCharacter(invitedCharacterName, true);
            if (existingOther != null)
                return false;

            if (existing == null)
            {
                RemovePending(characterName);
                existing = new PartyMemberEntity()
                {
                    PartyId = StringUtils.UniqueId(),
                    LeaderName = characterName,
                    MemberName = characterName,
                    HasJoined = true
                };
                existing.Save();
                SendMessageToPlayer(characterName, PartyMessageType.Private_PartyCreated);
            }

            var requestedJoin = GetByCharacterAndLeader(invitedCharacterName, characterName);
            if (requestedJoin.IsRequested)
            {
                RemovePending(invitedCharacterName);
                requestedJoin.PartyId = existing.PartyId;
                requestedJoin.HasJoined = true;
                requestedJoin.IsInvited = false;
                requestedJoin.IsRequested = false;
                requestedJoin.Save();
                SendMessageToPlayer(characterName, PartyMessageType.Private_JoinedParty);
                SendMessageToParty(requestedJoin.PartyId, PartyMessageType.PlayerJoined, characterName);
                return true;
            }

            var other = new PartyMemberEntity()
            {
                PartyId = existing.PartyId,
                MemberName = invitedCharacterName,
                LeaderName = characterName,
                IsInvited = true
            };
            other.Save();
            SendMessageToPlayer(invitedCharacterName, PartyMessageType.Private_InvitedToParty, characterName);
            SendMessageToPlayer(characterName, PartyMessageType.Private_PlayerInvited, invitedCharacterName);

            return true;
        }

        public bool JoinParty(string characterName, string otherName)
        {
            var character = GetCharacter(characterName);
            if (character == null) return false;
            var otherMember = GetMemberByCharacter(otherName, true);
            if (otherMember == null) return false;

            var members = GetMembersByLeader(otherMember.LeaderName);
            if (members.Count >= PartyMemberMax)
            {
                SendMessageToPlayer(characterName, PartyMessageType.PartyFull);
                return false;
            }

            var member = GetByCharacterAndLeader(characterName, otherMember.LeaderName);
            // rather than request, we will just join the party if invited
            if (member != null && member.IsInvited)
            {
                RemovePending(characterName);
                member.HasJoined = true;
                member.IsInvited = false;
                member.IsRequested = false;
                member.Save();
                SendMessageToPlayer(characterName, PartyMessageType.Private_JoinedParty);
                SendMessageToParty(member.PartyId, PartyMessageType.PlayerJoined, characterName);
                return true;
            }

            if (member == null)
                member = new PartyMemberEntity()
                {
                    PartyId = otherMember.PartyId,
                    MemberName = characterName,
                    LeaderName = otherMember.LeaderName
                };
            member.IsRequested = true;
            member.Save();
            SendMessageToPlayer(otherMember.LeaderName, PartyMessageType.Private_PlayerRequestedInvite, character.Name);
            SendMessageToPlayer(characterName, PartyMessageType.Private_RequestedInvite, character.Name);
            return true;
        }

        public bool LeaveParty(string characterName, bool disband = false)
        {
            var character = GetCharacter(characterName);
            if (character == null) return false;
            var member = GetJoinedParty(characterName);
            if (member == null) return false;
            SendMessageToPlayer(characterName, PartyMessageType.Private_LeftParty);
            member.Delete();
            SendMessageToParty(member.PartyId, PartyMessageType.PlayerLeft, characterName);
            if (member.LeaderName != member.MemberName) return true;
            var party = GetMembersByLeader(characterName);
            if (disband)
            {
                foreach (var m in party)
                {
                    if (m.MemberName == characterName) continue;
                    m.Delete();
                    SendMessageToParty(member.PartyId, PartyMessageType.PartyDisbanded, characterName);
                    break;
                }

                return true;
            }

            PartyMemberEntity firstNonLeader = null;
            foreach (var m in party)
            {
                if (m.MemberName == characterName) continue;
                firstNonLeader = m;
                break;
            }

            if (firstNonLeader == null) return true;
            foreach (var m in party)
            {
                m.LeaderName = firstNonLeader.MemberName;
                m.Save();
            }

            SendMessageToParty(member.PartyId, PartyMessageType.LeaderChanged, firstNonLeader.MemberName);
            return true;
        }

        public PartyModel GetParty(string characterName)
        {
            var character = GetCharacter(characterName);
            if (character == null) return null;
            var member = GetJoinedParty(characterName);
            if (member == null) return null;
            var members = GetCurrentAndPendingMembersByLeader(member.LeaderName);
            var party = new PartyModel();
            foreach (var m in members)
            {
                party.PartyId = m.PartyId;
                party.Leader = m.LeaderName;
                if (m.HasJoined)
                    party.Members.Add(m.MemberName);
                if (m.IsInvited)
                    party.Invitations.Add(m.MemberName);
                if (m.IsRequested)
                    party.Requests.Add(m.MemberName);
            }

            return party;
        }

        public static List<PartyMemberEntity> GetMembersByLeader(string character)
        {
            return DB.TakeAll<PartyMemberEntity>().Filter((p) => p.LeaderName == character && p.HasJoined).Get();
        }

        public static List<PartyMemberEntity> GetCurrentAndPendingMembersByLeader(string character)
        {
            return DB.TakeAll<PartyMemberEntity>().Filter((p) => p.LeaderName == character).Get();
        }

        public static PartyMemberEntity GetByCharacterAndLeader(string character, string leader)
        {
            return DB.TakeAll<PartyMemberEntity>().Filter((p) => p.MemberName == character && p.LeaderName == leader)
                .First();
        }

        public static PartyMemberEntity GetMemberByCharacter(string character, bool hasJoined)
        {
            return DB.TakeAll<PartyMemberEntity>().Filter((p) => p.MemberName == character && p.HasJoined == hasJoined)
                .First();
        }

        public static PartyMemberEntity GetJoinedParty(string character)
        {
            return DB.TakeAll<PartyMemberEntity>().Filter((p) => p.MemberName == character && p.HasJoined).First();
        }

        public static List<PartyMemberEntity> GetPendingByCharacter(string character)
        {
            return DB.TakeAll<PartyMemberEntity>().Filter((p) => p.MemberName == character && !p.HasJoined).Get();
        }

        public static void SendMessageToParty(string partyId, PartyMessageType type, string characterName)
        {
            Broadcast.Channel<PartyChannel>()
                .WithParameters(partyId)
                .Send(new PartyMessage()
                {
                    type = type,
                    characterName = characterName
                });
        }

        public static void SendMessageToPlayer(string toName, PartyMessageType type, string characterName = null,
            string partyId = null)
        {
            Broadcast.Channel<PrivateMessageChannel>()
                .WithParameters(toName)
                .Send(new PartyMessage()
                {
                    type = type,
                    characterName = characterName,
                    partyId = partyId
                });
        }
    }
}