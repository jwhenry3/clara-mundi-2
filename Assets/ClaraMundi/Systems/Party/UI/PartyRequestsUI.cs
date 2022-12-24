using System.Collections.Generic;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace ClaraMundi
{
    public class PartyRequestsUI : PlayerUI
    {
        public Transform RequestsContainer;

        public PartyJoinRequestUI JoinPrefab;
        public PartyInviteRequestUI InvitePrefab;

        private readonly Dictionary<string, PartyInviteRequestUI> Invites = new();
        private readonly Dictionary<string, PartyJoinRequestUI> JoinRequests = new();

        protected override void OnPlayerChange(Player _player)
        {
            if (player != null)
            {
                player.Party.InviteChanges -= OnInviteChanges;
                player.Party.PartyChanges -= OnPartyChanges;
            }
            Clear();
            base.OnPlayerChange(_player);
            if (player == null) return;
            Populate();
            player.Party.InviteChanges += OnInviteChanges;
            player.Party.PartyChanges += OnPartyChanges;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (player != null)
            {
                player.Party.InviteChanges -= OnInviteChanges;
                player.Party.PartyChanges -= OnPartyChanges;
            }
        }

        private void OnInviteChanges(List<string> invites)
        {
            var clone = new Dictionary<string, PartyInviteRequestUI>(Invites);
            foreach (var kvp in clone)
            {
                if (!invites.Contains(kvp.Key))
                    RemoveInvite(kvp.Key);
            }

            foreach (var i in invites)
            {
                if (!clone.ContainsKey(i))
                    AddInvite(i);
            }

            HandleVisibility();
        }

        private void OnPartyChanges(PartyModel party)
        {
            // only party leader can invite and accept requests
            if (party != null && party.Leader != PlayerManager.Instance.LocalPlayer.entityId)
            {
                // Any requests are cleaned up if the player is not the leader
                // in case the leadership changes during party existence
                Clear();
                HandleVisibility();
                return;
            }

            var clone = new Dictionary<string, PartyJoinRequestUI>(JoinRequests);
            foreach (var kvp in clone)
            {
                if (party != null && party.Requests.Contains(kvp.Key)) continue;
                // remove join request UI no longer in party details
                Destroy(kvp.Value.gameObject);
                JoinRequests.Remove(kvp.Key);
            }

            if (party == null) return;
            // add remaining join requests not currently in UI
            foreach (string joiningPlayerId in party.Requests)
                AddJoinRequest(joiningPlayerId);
            HandleVisibility();
        }

        public void Clear()
        {
            foreach (var kvp in Invites)
                Destroy(kvp.Value.gameObject);
            foreach (var kvp in JoinRequests)
                Destroy(kvp.Value.gameObject);
            Invites.Clear();
            JoinRequests.Clear();
        }

        public void Populate()
        {
            foreach (string invitingPlayerId in PlayerManager.Instance.LocalPlayer.Party.PartyInvites)
                AddInvite(invitingPlayerId);

            if (PlayerManager.Instance.LocalPlayer.Party.Party != null)
            {
                foreach (string joiningPlayerId in PlayerManager.Instance.LocalPlayer.Party.Party.Requests)
                    AddJoinRequest(joiningPlayerId);
            }

            HandleVisibility();
        }

        private void AddInvite(string invitingPlayerId)
        {
            if (Invites.ContainsKey(invitingPlayerId)) return;
            if (!PlayerManager.Instance.Players.ContainsKey(invitingPlayerId)) return;
            var instance = Instantiate(InvitePrefab, RequestsContainer, false);
            instance.invitingPlayerId = invitingPlayerId;
            instance.PlayerName.text = PlayerManager.Instance.Players[invitingPlayerId].Entity.entityName;
            Invites.Add(invitingPlayerId, instance);
        }

        private void RemoveInvite(string invitingPlayerId)
        {
            Destroy(Invites[invitingPlayerId]);
            if (!Invites.ContainsKey(invitingPlayerId)) return;
            Invites.Remove(invitingPlayerId);
        }

        private void AddJoinRequest(string joiningPlayerId)
        {
            if (JoinRequests.ContainsKey(joiningPlayerId)) return;
            if (!PlayerManager.Instance.Players.ContainsKey(joiningPlayerId)) return;
            var instance = Instantiate(JoinPrefab, RequestsContainer, false);
            instance.PlayerName.text = PlayerManager.Instance.Players[joiningPlayerId].Entity.entityName;
            instance.joiningPlayerId = joiningPlayerId;
            JoinRequests.Add(joiningPlayerId, instance);
        }

        private void HandleVisibility()
        {
            gameObject.SetActive(JoinRequests.Count > 0 || Invites.Count > 0);
        }
    }
}