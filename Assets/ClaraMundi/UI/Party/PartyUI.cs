using System.Collections.Generic;
using System.Linq;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine;

namespace ClaraMundi
{
    public class PartyUI : PlayerUI
    {
        private Party Party;
        public Transform PartyContainer;
        public PartyMemberUI PartyMemberPrefab;

        private string joiningLeaderId;
        public GameObject InvitedDialog;
        public GameObject InviteDialog;
        public TMP_InputField InviteField;
        
        protected override void OnPlayerChange(Player _player)
        {
            if (player != null)
            {
                player.Party.PartyChanges -= OnPartyChanges;
                player.Party.InviteChanges -= OnInviteChanges;
            }
            base.OnPlayerChange(_player);
            if (player != null)
            {
                player.Party.PartyChanges += OnPartyChanges;
                player.Party.InviteChanges += OnInviteChanges;
                OnPartyChanges(player.Party.Party);
            }
        }

        private void OnInviteChanges(SyncList<string> leaderIds)
        {
            if (leaderIds.Count > 0)
            {
                joiningLeaderId = leaderIds.Last();
                InvitedDialog.SetActive(true);
            }
            else
            {
                joiningLeaderId = null;
                InvitedDialog.SetActive(false);
            }
        }
        private void OnPartyChanges(Party party)
        {
            Party = party;
            if (party == null)
            {
                foreach (PartyMemberUI member in PartyContainer.GetComponentsInChildren<PartyMemberUI>())
                        member.SetPartyMember(null);
                return;
            }
            List<string> found = new();
            foreach (PartyMemberUI member in PartyContainer.GetComponentsInChildren<PartyMemberUI>())
            {
                if (party.MemberIds.Contains(member.player.entityId))
                {
                    member.SetPartyMember(member.player.entityId);
                    found.Add(member.player.entityId);
                }
                else
                    member.SetPartyMember(null);
            }

            foreach (string member in party.MemberIds)
            {
                if (found.Contains(member)) continue;
                var instance = Instantiate(PartyMemberPrefab, PartyContainer, false);
                instance.SetPartyMember(member);
            }
        }

        public void CreateParty()
        {
            if (Party == null)
                PlayerManager.Instance.LocalPlayer.Party.CreateParty();
            InviteDialog.SetActive(true);
        }

        public void InviteToParty()
        {
            if (!PlayerManager.Instance.PlayersByName.ContainsKey(InviteField.text.ToLower())) return;
            var pendingMember = PlayerManager.Instance.PlayersByName[InviteField.text.ToLower()];
            PlayerManager.Instance.LocalPlayer.Party.InviteToParty(pendingMember.entityId);
            InviteDialog.SetActive(false);
            InviteField.text = "";
        }

        public void JoinParty()
        {
            InvitedDialog.SetActive(false);
            if (string.IsNullOrEmpty(joiningLeaderId)) return;
            PlayerManager.Instance.LocalPlayer.Party.JoinParty(joiningLeaderId);
        }

        public void Decline()
        {
            InvitedDialog.SetActive(false);
            if (string.IsNullOrEmpty(joiningLeaderId)) return;
            PlayerManager.Instance.LocalPlayer.Party.DeclineInvite(joiningLeaderId);
        }

        public void LeaveParty()
        {
            PlayerManager.Instance.LocalPlayer.Party.LeaveParty();
        }

        public void CancelInvite()
        {
            InviteDialog.SetActive(false);
            InviteField.text = "";
        }
    }
}