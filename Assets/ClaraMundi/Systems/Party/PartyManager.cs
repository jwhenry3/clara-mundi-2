using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ClaraMundi
{
  public class PartyManager : MonoBehaviour
  {
    public static PartyManager Instance;
    public readonly Dictionary<string, Party> PartyByLeader = new();
    public readonly Dictionary<string, Party> Parties = new();

    private void Awake()
    {
      Instance = this;
    }

    public void CreateParty(string leader)
    {
      if (!PlayerExists(leader)) return;
      if (Parties.ContainsKey(leader)) return;
      Parties[leader] = new Party()
      {
        leader = leader,
        members = new List<string>() { leader }
      };
      PartyByLeader[leader] = Parties[leader];
      UpdateParty(Parties[leader]);
      var _player = GetPlayer(leader);
      _player.Party.CreatedParty(_player.Owner);
      // event
    }

    public void JoinParty(string player, string other)
    {
      if (!PlayerExists(player)) return;
      if (!PlayerExists(other)) return;
      if (Parties.ContainsKey(player)) return;
      if (!Parties.ContainsKey(other)) return;
      if (Parties[other].invited.Contains(player))
      {
        Parties[other].members.Add(player);
        Parties[player] = Parties[other];

        ClearRequests(player);
        foreach (var member in Parties[player].members)
        {
          var _member = GetPlayer(member);
          _member.Party.MemberJoined(_member.Owner, player);
        }

        // event
        return;
      }

      Parties[other].requests.Add(player);
      UpdateParty(Parties[other]);
      var _player = GetPlayer(Parties[other].leader);
      _player.Party.PlayerRequestedInvite(_player.Owner, player);
      // event
    }

    public void AcceptRequest(string player, string requestor)
    {
      if (!PlayerExists(player)) return;
      if (!PlayerExists(requestor)) return;
      if (Parties.ContainsKey(requestor)) return;
      if (!Parties.ContainsKey(player)) return;
      if (!Parties[player].requests.Contains(requestor)) return;
      Parties[player].members.Add(requestor);
      Parties[requestor] = Parties[player];
      ClearRequests(requestor);
      UpdateParty(Parties[player]);
      foreach (var member in Parties[player].members)
      {
        var _player = GetPlayer(member);
        _player.Party.MemberJoined(_player.Owner, player);
      }
      // event
    }

    public void DeclineRequest(string player, string requestor)
    {
      if (!PlayerExists(player)) return;
      if (!PlayerExists(requestor)) return;
      if (!Parties.ContainsKey(player)) return;
      if (!Parties[player].requests.Contains(requestor)) return;
      Parties[player].requests.Remove(requestor);

      var _player = GetPlayer(requestor);
      _player.Party.PlayerDeclinedInvite(_player.Owner, player);
      UpdateParty(Parties[player]);
      // event
    }

    public void DeclineInvite(string player, string leader)
    {
      if (!PlayerExists(player)) return;
      if (!PlayerExists(leader)) return;
      if (!Parties.ContainsKey(leader)) return;
      if (!Parties[leader].invited.Contains(player)) return;
      Parties[player].invited.Remove(player);
      var _player = GetPlayer(leader);
      _player.Party.PlayerDeclinedInvite(_player.Owner, player);
      UpdateParty(Parties[leader]);
      // event
    }

    public void InviteToParty(string player, string invited)
    {
      if (!PlayerExists(player)) return;
      if (!PlayerExists(invited)) return;
      if (Parties.ContainsKey(invited)) return;
      if (!Parties.ContainsKey(player)) return;
      if (Parties[player].requests.Contains(invited))
      {
        AcceptRequest(player, invited);
        return;
      }

      if (Parties[player].invited.Contains(invited)) return;
      Parties[player].invited.Add(invited);
      var _invited = GetPlayer(invited);
      var _player = GetPlayer(player);
      _invited.Party.PartyInvites.Add(player);
      _invited.Party.InvitedToParty(_invited.Owner, player);
      _player.Party.PlayerInvited(_player.Owner, invited);
      UpdateParty(Parties[player]);
      // event
    }

    public void LeaveParty(string player)
    {
      if (!PlayerExists(player)) return;
      if (!Parties.ContainsKey(player)) return;
      var party = Parties[player];
      if (party.members.Count == 1)
      {
        DisbandParty(player);
        return;
      }

      party.members.Remove(player);
      var _player = GetPlayer(player);
      _player.Party.YouLeft(_player.Owner);
      foreach (var member in party.members)
      {
        var _member = GetPlayer(member);
        _member.Party.MemberLeft(_member.Owner, player);
      }

      // event
      if (party.leader == player)
      {
        var newLeader = party.members.First();
        Promote(player, newLeader);
      }
      UpdateParty(party);
      // event
    }

    public void Promote(string leader, string player)
    {
      if (!PlayerExists(leader)) return;
      if (!PlayerExists(player)) return;
      if (!Parties.ContainsKey(leader)) return;
      if (!Parties.ContainsKey(player)) return;
      if (!PartyByLeader.ContainsKey(leader)) return;
      LeaveParty(player);
    }

    public void Kick(string leader, string player)
    {
      if (!PlayerExists(leader)) return;
      if (!PlayerExists(player)) return;
      if (!Parties.ContainsKey(leader)) return;
      if (!Parties.ContainsKey(player)) return;
      if (!PartyByLeader.ContainsKey(leader)) return;
    }

    public void DisbandParty(string player)
    {
      if (!PlayerExists(player)) return;
      if (!Parties.ContainsKey(player)) return;
      var party = Parties[player];
      if (party.leader != player) return;
      foreach (var member in party.members)
      {
        var _player = GetPlayer(member);
        _player.Party.DisbandedParty(_player.Owner);
        _player.Party.Party.Value = null;
        Parties.Remove(member);
      }

      PartyByLeader.Remove(player);
      // event
    }

    public void ClearRequests(string requestPlayer)
    {
      if (!PlayerExists(requestPlayer)) return;
      var player = GetPlayer(requestPlayer);
      player.Party.PartyInvites.Clear();
      foreach (var kvp in PartyByLeader)
      {
        var changed = false;
        if (kvp.Value.invited.Contains(requestPlayer))
        {
          kvp.Value.invited.Remove(requestPlayer);
          changed = true;
        }

        if (kvp.Value.requests.Contains(requestPlayer))
        {
          kvp.Value.requests.Remove(requestPlayer);
          changed = true;
        }

        if (changed)
        {
          UpdateParty(kvp.Value);
        }
        // event
      }
    }

    private void UpdateParty(Party party)
    {
      Parties[party.leader] = new Party()
      {
        leader = party.leader,
        members = party.members,
        invited = party.invited,
        requests = party.requests,
      };
      foreach (var member in party.members)
      {
        if (Parties.ContainsKey(member) && PlayerManager.Instance.PlayersByName.ContainsKey(member))
          GetPlayer(member).Party.Party.Value = party;
      }
    }
    bool PlayerExists(string playerName)
    {
      return PlayerManager.Instance.PlayersByName.ContainsKey(playerName);
    }
    Player GetPlayer(string playerName)
    {
      return PlayerManager.Instance.PlayersByName[playerName];
    }

    public void SendChatMessage(ChatMessage message)
    {
      var player = message.SenderCharacterName;
      if (!Parties.ContainsKey(player)) return;
      foreach (var member in Parties[player].members)
      {
        var _member = GetPlayer(member);
        _member.Party.LastMessage.Value = message;
      }
    }
  }
}