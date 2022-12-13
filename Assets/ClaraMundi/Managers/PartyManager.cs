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
            var player = GetPlayer(playerId);
            player.Party.PartyInvites.Clear();
            CleanUpPendingPlayer(playerId);
            UpdateParty(party);

            return false;
        }

        public bool ServerDecline(string playerId, string leaderPlayerId)
        {
            if (!PartiesByLeader.TryGetValue(leaderPlayerId, out var party)) return false;
            if (party.InvitedIds.Contains(playerId))
            {
                party.InvitedIds.Remove(playerId);
                GetPlayer(playerId).Party.PartyInvites.Remove(leaderPlayerId);
                return true;
            }

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
            {
                PlayerManager.Instance.Players[member].Party.Server_OnChatMessage(message);
            }
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
            {
                if (party.established)
                    PlayerManager.Instance.Players[member].Party.Server_OnChange(newParty);
                else
                    PlayerManager.Instance.Players[member].Party.Server_OnChange(null);
            }

            if (party.established) return;
            CleanUpParty(party);
        }

        void CleanUpParty(Party party)
        {
            foreach (string playerId in party.InvitedIds)
                GetPlayer(playerId).Party.PartyInvites.Remove(party.LeaderId);
            party.RequestedjoinerIds.Clear();
            PartiesByLeader.Remove(party.LeaderId);
        }

        void CleanUpPendingPlayer(string playerId)
        {
            var clone = new Dictionary<string, Party>(PartiesByLeader);
            foreach (var kvp in clone)
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