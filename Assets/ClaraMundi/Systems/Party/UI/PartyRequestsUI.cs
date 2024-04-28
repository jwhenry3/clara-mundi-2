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

    public override void Start()
    {
      base.Start();
      foreach (PartyJoinRequestUI child in RequestsContainer.GetComponentsInChildren<PartyJoinRequestUI>())
      {
        Destroy(child.gameObject);
      }
      foreach (PartyInviteRequestUI child in RequestsContainer.GetComponentsInChildren<PartyInviteRequestUI>())
      {
        Destroy(child.gameObject);
      }
    }
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
      Debug.Log(invites.Count);
      var clone = new Dictionary<string, PartyInviteRequestUI>(Invites);
      foreach (var kvp in clone)
      {
        if (!invites.Contains(kvp.Key))
          RemoveInvite(kvp.Key);
      }

      foreach (string i in invites)
      {
        if (!clone.ContainsKey(i))
          AddInvite(i);
      }
    }

    private void OnPartyChanges(Party party)
    {
      // only party leader can invite and accept requests
      if (party != null && party.leader != PlayerManager.Instance.LocalPlayer.Character.name)
      {
        // Any requests are cleaned up if the player is not the leader
        // in case the leadership changes during party existence
        Clear();
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
      var PartySystem = PlayerManager.Instance.LocalPlayer.Party;
      foreach (string invitingPlayerId in PartySystem.PartyInvites)
        AddInvite(invitingPlayerId);

      if (PartySystem.Party.Value != null)
      {
        foreach (string joiningPlayerId in PartySystem.Party.Value.requests)
          AddJoinRequest(joiningPlayerId);
      }
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

  }
}