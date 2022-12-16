using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ClaraMundi
{
    public class PartyManager : MonoBehaviour
    {
        public readonly Dictionary<string, Party> PartiesByLeader = new();
        public static PartyManager Instance;
        public int PartyMemberLimit = 8;
        private void Awake()
        {
            Instance = this;
        }

        Player GetPlayer(string playerId)
        {
            return !PlayerManager.Instance.Players.ContainsKey(playerId) ? null : PlayerManager.Instance.Players[playerId];
        }

        bool IsPlayerInParty(string playerId)
        {
            return PartiesByLeader.Any(kvp => kvp.Value.MemberIds.Contains(playerId));
        }

        Party GetPlayerParty(string playerId)
        {
            return PartiesByLeader.FirstOrDefault(kvp => kvp.Value.MemberIds.Contains(playerId)).Value;
        }

        public bool ServerInviteToParty(string leaderPlayerId, string invitePlayerId)
        {
            if (IsPlayerInParty(invitePlayerId)) return false;
            if (!PartiesByLeader.TryGetValue(leaderPlayerId, out var party))
            {
                if (!ServerCreateParty(leaderPlayerId)) return false;
                if (!PartiesByLeader.TryGetValue(leaderPlayerId, out party)) return false;
            }
            if (party.MemberIds.Count >= PartyMemberLimit) return false;
            
            party.InvitedIds.Add(invitePlayerId);
            var invitedPlayer = GetPlayer(invitePlayerId);
            invitedPlayer.Party.PartyInvites.Add(leaderPlayerId);
            UpdateParty(party);
            
            return true;
        }
        
        public bool ServerRequestPartyJoin(string playerId, string leaderPlayerId)
        {
            if (IsPlayerInParty(playerId)) return false;
            if (!PartiesByLeader.TryGetValue(leaderPlayerId, out var party)) return false;
            if (party.MemberIds.Count >= PartyMemberLimit) return false;
            
            party.RequestedjoinerIds.Add(playerId);
            var player = GetPlayer(leaderPlayerId);
            player.Party.Server_OnChange(party);
            UpdateParty(party);
            
            return true;
        }

        public bool AcceptRequest(string leaderPlayerId, string playerId)
        {
            if (IsPlayerInParty(playerId)) return false;
            if (!PartiesByLeader.TryGetValue(leaderPlayerId, out var party)) return false;
            if (!party.RequestedjoinerIds.Contains(playerId) || party.MemberIds.Contains(playerId)) return false;
            return Joined(playerId, party);
        }
        public bool ServerJoinParty(string playerId, string leaderPlayerId)
        {
            if (IsPlayerInParty(playerId)) return false;
            if (!PartiesByLeader.TryGetValue(leaderPlayerId, out var party)) return false;
            if (!party.InvitedIds.Contains(playerId) || party.MemberIds.Contains(playerId)) return false;
            return Joined(playerId, party);
        }

        private bool Joined(string playerId, Party party)
        {
            if (party.MemberIds.Count >= PartyMemberLimit)
            {
                party.InvitedIds.Remove(playerId);
                UpdateParty(party);
                return false;
            }
            party.MemberIds.Add(playerId);
            CleanUpPendingPlayer(playerId);
            if (party.MemberIds.Count >= PartyMemberLimit)
                CleanUpPendingParty(party);
            UpdateParty(party);

            return true;
        }

        public bool ServerDecline(string playerId, string leaderPlayerId)
        {
            if (!PartiesByLeader.TryGetValue(leaderPlayerId, out var party)) return false;
            var player = GetPlayer(playerId);
            if (player.Party.PartyInvites.Contains(leaderPlayerId))
                player.Party.PartyInvites.Remove(leaderPlayerId);
            if (!party.InvitedIds.Contains(playerId)) return false;
            party.InvitedIds.Remove(playerId);
            UpdateParty(party);
            return true;

        }

        public bool ServerDeclineRequest(string playerId, string joiningPlayerId)
        {
            if (!PartiesByLeader.TryGetValue(playerId, out var party)) return false;
            if (!party.RequestedjoinerIds.Contains(joiningPlayerId)) return false;
            party.RequestedjoinerIds.Remove(joiningPlayerId);
            UpdateParty(party);
            return true;
        }


        public bool ServerCreateParty(string leaderPlayerId)
        {
            if (IsPlayerInParty(leaderPlayerId)) return false;
            var party = new Party
            {
                established =  true,
                LeaderId = leaderPlayerId,
            };
            party.MemberIds.Add(leaderPlayerId);
            CleanUpPendingPlayer(leaderPlayerId);
            UpdateParty(party);
            return true;
        }

        public void ServerDisbandParty(string leaderPlayerId)
        {
            if (!IsPlayerInParty(leaderPlayerId)) return;
            var party = PartiesByLeader[leaderPlayerId];
            party.established = false;
            UpdateParty(party);
        }

        public void ServerLeaveParty(string playerId, string leaderPlayerId)
        {
            if (!IsPlayerInParty(playerId)) return;
            var party = PartiesByLeader[leaderPlayerId];

            if (party.MemberIds.Count == 1 && party.LeaderId == playerId)
            {
                ServerDisbandParty(playerId);
                return;
            }

            // Pass the leader status to the next player
            if (party.LeaderId  == playerId && party.MemberIds.Count > 0)
            {
                PartiesByLeader.Remove(leaderPlayerId);
                party.LeaderId = party.MemberIds[0];
                PartiesByLeader[party.MemberIds[0]] = party;
            }

            party.MemberIds.Remove(playerId);
            var player = GetPlayer(playerId);
            player.Party.Server_OnChange(null);

            UpdateParty(party);
        }

        public void ServerSendMessage(ChatMessage message)
        {
            var party = GetPlayerParty(message.SenderEntityId);
            if (party == null) return;
            foreach (string member in party.MemberIds)
                PlayerManager.Instance.Players[member].Party.Server_OnChatMessage(message);
        }
        
        void UpdateParty(Party party)
        {
            var newParty = new Party
            {
                established = party.established,
                MemberIds = party.MemberIds,
                LeaderId = party.LeaderId,
                InvitedIds = party.InvitedIds,
                RequestedjoinerIds = party.RequestedjoinerIds
            };
            PartiesByLeader[party.LeaderId] = newParty;
            foreach (string member in party.MemberIds)
                PlayerManager.Instance.Players[member].Party.Server_OnChange(party.established ? newParty : null);

            if (party.established) return;
            CleanUpParty(party);
        }

        void CleanUpPendingParty(Party party)
        {
            foreach (string invitedId in party.InvitedIds)
                GetPlayer(invitedId).Party.PartyInvites.Remove(party.LeaderId);
            party.RequestedjoinerIds.Clear();
            party.InvitedIds.Clear();
        }
        void CleanUpParty(Party party)
        {
            foreach (string playerId in party.InvitedIds)
                GetPlayer(playerId).Party.PartyInvites.Remove(party.LeaderId);
            party.RequestedjoinerIds.Clear();
            party.InvitedIds.Clear();
            PartiesByLeader.Remove(party.LeaderId);
        }

        void CleanUpPendingPlayer(string playerId)
        {
            var clone = new Dictionary<string, Party>(PartiesByLeader);
            var player = GetPlayer(playerId);
            player.Party.PartyInvites.Clear();
            foreach (var kvp in clone)
            {
                if (!kvp.Value.InvitedIds.Contains(playerId) && !kvp.Value.RequestedjoinerIds.Contains(playerId))
                    continue;
                if (kvp.Value.InvitedIds.Contains(playerId))
                    kvp.Value.InvitedIds.Remove(playerId);
                if (kvp.Value.RequestedjoinerIds.Contains(playerId))
                    kvp.Value.RequestedjoinerIds.Remove(playerId);
                UpdateParty(kvp.Value);
            }
        }
    }
}