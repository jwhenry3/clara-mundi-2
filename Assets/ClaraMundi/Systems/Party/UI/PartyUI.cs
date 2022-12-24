using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ClaraMundi
{
    public class PartyUI : PlayerUI
    {
        private PartyModel Party;
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

        private void OnPartyChanges(PartyModel party)
        {
            Party = party;
            if (!PartyClient.DoesPartyExist(party))
            {
                foreach (PartyMemberUI member in PartyContainer.GetComponentsInChildren<PartyMemberUI>())
                        member.SetPartyMember(null);
                InviteButton.gameObject.SetActive(true);
                return;
            }
            InviteButton.gameObject.SetActive(party.Leader == player.entityId);
            List<string> found = new();
            foreach (PartyMemberUI member in PartyContainer.GetComponentsInChildren<PartyMemberUI>())
            {
                if (member.player != null && party.Members.Contains(member.player.Character.Name))
                {
                    member.SetPartyMember(member.player.Character.Name);
                    found.Add(member.player.Character.Name);
                    if (member.player.Character.Name == PlayerManager.Instance.LocalPlayer.Character.Name)
                        member.transform.SetAsFirstSibling();
                }
                else
                    member.SetPartyMember(null);
                
            }

            foreach (string member in party.Members)
            {
                if (found.Contains(member)) continue;
                var instance = Instantiate(PartyMemberPrefab, PartyContainer, false);
                instance.SetPartyMember(member);
                if (member == PlayerManager.Instance.LocalPlayer.Character.Name)
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
            PlayerManager.Instance.LocalPlayer.Party.InviteToParty(InviteField.text.ToLower());
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