using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ClaraMundi
{
    public class PartyUI : PlayerUI
    {
        private Party Party;
        public Transform PartyContainer;
        public PartyMemberUI PartyMemberPrefab;

        public GameObject InviteDialog;
        public TMP_InputField InviteField;
        
        protected override void OnPlayerChange(Player _player)
        {
            if (player != null)
            {
                player.Party.PartyChanges -= OnPartyChanges;
            }
            base.OnPlayerChange(_player);
            if (player != null)
            {
                player.Party.PartyChanges += OnPartyChanges;
                OnPartyChanges(player.Party.Party);
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