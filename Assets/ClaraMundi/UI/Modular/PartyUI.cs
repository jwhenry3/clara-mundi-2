using System;
using System.Collections.Generic;
using ReactUnity.UGUI.EventHandlers;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClaraMundi
{
  public class PartyUI : MonoBehaviour
  {

    private Player player;

    public WindowUI InviteWindow;
    public TMP_InputField InviteField;
    public WindowUI MemberActionMenu;

    public ButtonUI ActionCreate;
    public ButtonUI ActionKick;
    public ButtonUI ActionLeave;
    public ButtonUI ActionPromote;


    public GameObject NoPartyActions;
    public GameObject PartyMemberActions;
    public GameObject LeaderActions;
    public GameObject NoPartyMembers;
    public GameObject PartyMembersInMenu;
    public GameObject PartyMembersInHud;

    public PartyMemberUI PartyMemberMenuItemPrefab;
    public PartyMemberUI PartyMemberHudPrefab;

    public PartyMemberUI CurrentMember;

    public Party CurrentParty;

    private bool listening;

    void Start()
    {

    }
    void OnEnable()
    {
      if (!listening)
      {
        listening = true;
        player = PlayerManager.Instance.LocalPlayer;
        player.Party.PartyChanges += OnPartyChanges;
        OnPartyChanges(player.Party.Party.Value);
      }
    }
    void OnDisable()
    {
      if (listening)
      {
        listening = false;
        player.Party.PartyChanges -= OnPartyChanges;
      }
    }

    private void OnPartyChanges(Party party)
    {
      if (party == null)
      {
        NoPartyActions.SetActive(true);
        NoPartyMembers.SetActive(true);
        PartyMemberActions.SetActive(false);
        LeaderActions.SetActive(false);
        PartyMembersInMenu.SetActive(false);
      }
      else
      {
        NoPartyActions.SetActive(false);
        NoPartyMembers.SetActive(false);
        PartyMemberActions.SetActive(party.leader != player.entityId);
        LeaderActions.SetActive(party.leader == player.entityId);
        PartyMembersInMenu.SetActive(true);
      }
      CurrentParty = party;
      UpdatePartyList(party);
    }

    private void UpdatePartyList(Party party)
    {
      List<string> foundInHud = new();
      List<string> foundInMenu = new();

      foreach (PartyMemberUI member in PartyMembersInHud.transform.GetComponentsInChildren<PartyMemberUI>())
      {
        if (party == null || string.IsNullOrEmpty(member.playerName) || !party.members.Contains(member.playerName))
        {
          Destroy(member.gameObject);
          continue;
        }
        foundInHud.Add(member.playerName);
      }
      foreach (PartyMemberUI member in PartyMembersInMenu.transform.GetComponentsInChildren<PartyMemberUI>())
      {
        if (party == null || string.IsNullOrEmpty(member.playerName) || !party.members.Contains(member.playerName))
        {
          Destroy(member.gameObject);
          continue;
        }
        foundInMenu.Add(member.playerName);
      }

      if (party == null)
      {
        string name = player.Entity.entityName.Value;
        Instantiate(PartyMemberHudPrefab, PartyMembersInHud.transform, false).SetPartyMember(name);
        // Instantiate(PartyMemberMenuItemPrefab, PartyMembersInMenu.transform, false).SetPartyMember(name);
        return;
      }

      foreach (string member in party.members)
      {
        if (!foundInHud.Contains(member))
          Instantiate(PartyMemberHudPrefab, PartyMembersInHud.transform, false).SetPartyMember(member);
        if (!foundInMenu.Contains(member))
          Instantiate(PartyMemberMenuItemPrefab, PartyMembersInMenu.transform, false).SetPartyMember(member);
      }
    }

    public void CreateParty()
    {
      player.Party.CreateParty();
    }

    public void InviteToParty()
    {
      player.Party.InviteToParty(InviteField.text.ToLower());
      CloseInvite();
    }


    public void LeaveParty()
    {
      player.Party.LeaveParty();
    }
    public void DisbandParty()
    {
      player.Party.DisbandParty();
    }

    public void CloseInvite()
    {
      InviteWindow.moveSibling.ToBack();
      InviteField.text = "";
    }
    public void OpenMemberMenu(PartyMemberUI memberUI)
    {
      var playerName = player.Entity.entityName.Value;
      CurrentMember = memberUI;
      ActionCreate.gameObject.SetActive(memberUI.playerName == playerName && CurrentParty == null);
      ActionPromote.gameObject.SetActive(memberUI.playerName != playerName && CurrentParty != null && CurrentParty.leader == playerName);
      ActionLeave.gameObject.SetActive(memberUI.playerName == playerName && CurrentParty != null);
      ActionKick.gameObject.SetActive(memberUI.playerName != playerName && CurrentParty.leader == playerName);
      if (ActionCreate.gameObject.activeSelf)
        ActionCreate.AutoFocus = true;
      else if (ActionPromote.gameObject.activeSelf)
        ActionPromote.AutoFocus = true;
      else if (ActionLeave.gameObject.activeSelf)
        ActionLeave.AutoFocus = true;
      else if (ActionKick.gameObject.activeSelf)
        ActionKick.AutoFocus = true;
      MemberActionMenu.moveSibling.ToFront();
    }

    public void PromoteLeader()
    {
      player.Party.Promote(CurrentMember.playerName);
    }

    public void Kick()
    {
      player.Party.Kick(CurrentMember.playerName);
    }

    public void Whisper()
    {
      ChatUI.Instance.Focus("/tell " + CurrentMember.playerName + " ");
    }
  }
}