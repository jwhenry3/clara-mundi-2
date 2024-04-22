using System.Collections.Generic;
using ReactUnity.UGUI.EventHandlers;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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

    public ContextMenu PlayerContextMenu;
    public string ContextualCharacterName;
    public GameObject CreateButton;
    public GameObject CreateTopButton;
    public GameObject LeaveButton;
    public GameObject LeaveTopButton;
    public GameObject WhisperButton;
    public GameObject PromoteLeaderButton;
    public GameObject KickButton;

    public CanvasGroupFocus PartyMenuFocus;


    public PartyMemberUI PartyMemberMenuItemPrefab;

    public GameObject InviteDialog;
    public TMP_InputField InviteField;

    public Button InviteButton;

    public override void Start()
    {
      base.Start();
      PartyMenuFocus ??= GetComponent<CanvasGroupFocus>();
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

      CreateTopButton.SetActive(Party == null);
      LeaveTopButton.SetActive(Party != null);
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
      PlayerManager.Instance.LocalPlayer.Party.CreateParty();
      Refocus();
      InviteDialog.SetActive(true);
    }

    public void InviteToParty()
    {
      PlayerManager.Instance.LocalPlayer.Party.InviteToParty(InviteField.text.ToLower());
      Refocus();
    }


    public void LeaveParty()
    {
      PlayerManager.Instance.LocalPlayer.Party.LeaveParty();
      Refocus();
    }

    public void CancelInvite()
    {
      InviteDialog.SetActive(false);
      InviteField.text = "";
      Refocus();
    }


    public void OpenPlayerContextMenu(Vector3 position, string characterName)
    {
      PlayerContextMenu.ContextualText = characterName;
      var myName = PlayerManager.Instance.LocalPlayer.Character.name;
      PlayerContextMenu.transform.position = position;
      CreateButton.SetActive(Party == null);
      LeaveButton.SetActive(Party != null && characterName == myName);
      PromoteLeaderButton.SetActive(Party != null && Party.leader == myName && characterName != myName);
      WhisperButton.SetActive(characterName != myName);
      KickButton.SetActive(Party != null && Party.leader == myName && characterName != myName);
      if (Party == null || characterName != myName)
      {
        PlayerContextMenu.gameObject.SetActive(true);
        PlayerContextMenu.SelectFirstElement();
      }

    }

    public void PromoteLeader()
    {
      PlayerManager.Instance.LocalPlayer.Party.Promote(ContextualCharacterName);
      Refocus();
    }

    public void Kick()
    {
      PlayerManager.Instance.LocalPlayer.Party.Kick(ContextualCharacterName);
      Refocus();
    }

    public void Whisper()
    {
      ChatWindowUI.Instance.ContextualCharacterName = PlayerContextMenu.ContextualText;
      ChatWindowUI.Instance.Whisper();
      PlayerContextMenu.ContextualText = "";
      PlayerContextMenu.gameObject.SetActive(false);
      InviteDialog.SetActive(false);
      InviteField.text = "";
    }

    public void Refocus()
    {
      ContextualCharacterName = null;
      PartyMenuFocus.Select();
      PlayerContextMenu.ContextualGameObject = null;
      PlayerContextMenu.gameObject.SetActive(false);
      InviteDialog.SetActive(false);
      InviteField.text = "";
    }
  }
}