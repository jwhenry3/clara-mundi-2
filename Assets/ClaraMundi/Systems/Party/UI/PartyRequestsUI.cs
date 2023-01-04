using System.Collections.Generic;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace ClaraMundi
{
    public class PartyRequestsUI : PlayerUI
    {
        public Transform Panel;
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

        private void OnInviteChanges(SyncList<string> invites)
        {
            Debug.Log("Update invite list!");
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

        private void OnPartyChanges(Party party)
        {
            // only party leader can invite and accept requests
            if (party != null && party.leader != PlayerManager.Instance.LocalPlayer.Character.name)
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
                if (party != null && party.requests.Contains(kvp.Key)) continue;
                // remove join request UI no longer in party details
                Destroy(kvp.Value.gameObject);
                JoinRequests.Remove(kvp.Key);
            }

            if (party == null) return;
            // add remaining join requests not currently in UI
            foreach (string characterName in party.requests)
                AddJoinRequest(characterName);
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
                foreach (string joiningPlayerId in PlayerManager.Instance.LocalPlayer.Party.Party.requests)
                    AddJoinRequest(joiningPlayerId);
            }

            HandleVisibility();
        }

        private void AddInvite(string characterName)
        {
            if (Invites.ContainsKey(characterName)) return;
            var instance = Instantiate(InvitePrefab, RequestsContainer, false);
            instance.characterName = characterName;
            instance.PlayerName.text = characterName;
            Invites.Add(characterName, instance);
        }

        private void RemoveInvite(string characterName)
        {
            Destroy(Invites[characterName]);
            if (!Invites.ContainsKey(characterName)) return;
            Invites.Remove(characterName);
        }

        private void AddJoinRequest(string characterName)
        {
            if (JoinRequests.ContainsKey(characterName)) return;
            var instance = Instantiate(JoinPrefab, RequestsContainer, false);
            instance.PlayerName.text = characterName;
            instance.characterName = characterName;
            JoinRequests.Add(characterName, instance);
        }

        private void HandleVisibility()
        {
            Panel.gameObject.SetActive(JoinRequests.Count > 0 || Invites.Count > 0);
        }
    }
}