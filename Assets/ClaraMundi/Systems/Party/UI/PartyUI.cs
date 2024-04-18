using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ClaraMundi
{
  public class PartyUI : PlayerUI
  {
    public static PartyUI Instance;
    private Party Party;
    public Transform PartyContainer;
    public Transform PartyMenuContainer;
    public PartyMemberUI PartyMemberPrefab;

    public PartyMemberUI PartyMemberMenuItemPrefab;

    public GameObject InviteDialog;
    public TMP_InputField InviteField;

    public Button InviteButton;
    public Button LeaveButton;

    public override void Start()
    {
      base.Start();
      Instance = this;
    }

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
        OnPartyChanges(player.Party.Party.Value);
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
      List<string> foundInHud = new();
      List<string> foundInMenu = new();

      foreach (PartyMemberUI member in PartyContainer.GetComponentsInChildren<PartyMemberUI>())
      {
        if (party == null || !party.members.Contains(member.playerName))
        {
          Destroy(member.gameObject);
          continue;
        }
        foundInHud.Add(member.playerName);
      }
      foreach (PartyMemberUI member in PartyMenuContainer.GetComponentsInChildren<PartyMemberUI>())
      {
        if (party == null || !party.members.Contains(member.playerName))
        {
          Destroy(member.gameObject);
          continue;
        }
        foundInMenu.Add(member.playerName);
      }

      if (party == null)
      {
        string name = PlayerManager.Instance.LocalPlayer.Entity.entityName.Value;
        Instantiate(PartyMemberPrefab, PartyContainer, false).SetPartyMember(name);
        Instantiate(PartyMemberMenuItemPrefab, PartyMenuContainer, false).SetPartyMember(name);
        InviteButton.gameObject.SetActive(true);
        LeaveButton.gameObject.SetActive(false);
        return;
      }
      InviteButton.gameObject.SetActive(party.leader == player.Character.name);
      LeaveButton.gameObject.SetActive(true);

      foreach (string member in party.members)
      {
        if (!foundInHud.Contains(member))
          Instantiate(PartyMemberPrefab, PartyContainer, false).SetPartyMember(member);
        if (!foundInMenu.Contains(member))
          Instantiate(PartyMemberMenuItemPrefab, PartyMenuContainer, false).SetPartyMember(member);
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