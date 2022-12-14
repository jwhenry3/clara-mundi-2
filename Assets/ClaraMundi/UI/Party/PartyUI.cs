using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ClaraMundi
{
    public class PartyUI : PlayerUI
    {
        private Party Party;
        public Transform PartyContainer;
        public PartyMemberUI PartyMemberPrefab;

        private string joiningLeaderId;
        public GameObject InviteDialog;
        public TMP_InputField InviteField;

        public Button InviteButton;
        
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
        public override void OnDestroy()
        {
            base.OnDestroy();
            if (player != null)
            {
                player.Party.PartyChanges -= OnPartyChanges;
            }
        }

        private void OnPartyChanges(Party party)
        {
            Party = party;
            if (party == null || party.established == false)
            {
                foreach (PartyMemberUI member in PartyContainer.GetComponentsInChildren<PartyMemberUI>())
                        member.SetPartyMember(null);
                InviteButton.gameObject.SetActive(true);
                return;
            }
            InviteButton.gameObject.SetActive(party.LeaderId == player.entityId);
            List<string> found = new();
            foreach (PartyMemberUI member in PartyContainer.GetComponentsInChildren<PartyMemberUI>())
            {
                if (party.MemberIds.Contains(member.player.entityId))
                {
                    member.SetPartyMember(member.player.entityId);
                    found.Add(member.player.entityId);
                    if (member.player.entityId == PlayerManager.Instance.LocalPlayer.entityId)
                        member.transform.SetAsFirstSibling();
                }
                else
                    member.SetPartyMember(null);
                
            }

            foreach (string member in party.MemberIds)
            {
                if (found.Contains(member)) continue;
                var instance = Instantiate(PartyMemberPrefab, PartyContainer, false);
                instance.SetPartyMember(member);
                if (member == PlayerManager.Instance.LocalPlayer.entityId)
                    instance.transform.SetAsFirstSibling();
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