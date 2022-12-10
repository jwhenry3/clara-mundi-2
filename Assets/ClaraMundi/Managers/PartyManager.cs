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
            
            CleanUpPendingPlayer(playerId);
            party.RequestedjoinerIds.Add(playerId);
            var player = GetPlayer(leaderPlayerId);
            player.Party.Server_OnChange(party);
            
            return true;
        }

        public bool ServerJoinParty(string playerId, string leaderPlayerId)
        {
            if (IsPlayerInParty(playerId)) return false;
            if (!PartiesByLeader.TryGetValue(leaderPlayerId, out var party)) return false;
            if (party.MemberIds.Count >= PartyMemberLimit) return false;
            if (!party.InvitedIds.Contains(playerId) || party.MemberIds.Contains(playerId)) return false;
            party.MemberIds.Add(playerId);
            CleanUpPendingPlayer(playerId);
            var player = GetPlayer(playerId);
            player.Party.PartyInvites.Clear();
            UpdateParty(party);

            return false;
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
            PartiesByLeader.Add(leaderPlayerId, party);
            CleanUpPendingPlayer(leaderPlayerId);
            return true;
        }

        public void ServerDisbandParty(string leaderPlayerId)
        {
            if (!IsPlayerInParty(leaderPlayerId)) return;
            var party = PartiesByLeader[leaderPlayerId];
            party.established = false;
            foreach (string playerId in party.InvitedIds)
            {
                var player = GetPlayer(playerId);
                player.Party.PartyInvites.Remove(leaderPlayerId);
            }
            UpdateParty(party);
            PartiesByLeader.Remove(leaderPlayerId);
        }

        public void ServerLeaveParty(string playerId, string leaderPlayerId)
        {
            if (!IsPlayerInParty(playerId)) return;
            var party = PartiesByLeader[leaderPlayerId];
            // Disband if you're the only player in the party
            if (party.MemberIds.Count == 1 && party.LeaderId == playerId)
            {
                ServerDisbandParty(leaderPlayerId);
                return;
            }
            party.MemberIds.Remove(playerId);
            // Pass the leader status to the next player
            if (party.LeaderId  == playerId)
            {
                PartiesByLeader.Remove(leaderPlayerId);
                party.LeaderId = party.MemberIds[0];
                PartiesByLeader[party.MemberIds[0]] = party;
            }
            var player = GetPlayer(playerId);
            player.Party.Server_OnChange(null);
            UpdateParty(party);
        }

        void UpdateParty(Party party)
        {
            foreach (string member in party.MemberIds)
            {
                PlayerManager.Instance.Players[member].Party.Server_OnChange(party);
            }
        }

        void CleanUpPendingPlayer(string playerId)
        {
            foreach (var kvp in PartiesByLeader)
            {
                bool changed = false;
                if (kvp.Value.InvitedIds.Contains(playerId))
                {
                    kvp.Value.InvitedIds.Remove(playerId);
                    changed = true;
                }
                if (kvp.Value.RequestedjoinerIds.Contains(playerId))
                {
                    kvp.Value.RequestedjoinerIds.Remove(playerId);
                    changed = true;
                }
                if (changed)
                    UpdateParty(kvp.Value);
            }
        }
    }
}