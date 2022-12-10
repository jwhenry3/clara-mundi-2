﻿using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace ClaraMundi
{
    public class PartyController : PlayerController
    {
        [SyncObject]
        public readonly SyncList<string> PartyInvites = new();
        public event Action<Party> PartyChanges;
        public event Action<SyncList<string>> InviteChanges;
        
        [SyncVar(OnChange = "Client_OnChange")]
        public Party Party;

        protected override void Awake()
        {
            base.Awake();
            PartyInvites.OnChange += OnPartyInviteChange;
        }

        private void OnDestroy()
        {
            PartyInvites.OnChange -= OnPartyInviteChange;
        }

        private void OnPartyInviteChange(SyncListOperation op, int index, string previous, string next, bool asServer)
        {
            InviteChanges?.Invoke(PartyInvites);
        }

        public void Server_OnChange(Party party)
        {
                Party = party;
        }

        private void Client_OnChange(Party lastParty, Party nextParty, bool asServer)
        {
            PartyChanges?.Invoke(Party);
        }
        
        [ServerRpc]
        public void CreateParty()
        {
            PartyManager.Instance.ServerCreateParty(player.entityId);
        }
        [ServerRpc]
        public void DisbandParty()
        {
            PartyManager.Instance.ServerDisbandParty(player.entityId);
        }

        [ServerRpc]
        public void InviteToParty(string playerId)
        {
            PartyManager.Instance.ServerInviteToParty(player.entityId, playerId);
        }

        [ServerRpc]
        public void RequestJoin(string playerId)
        {
            PartyManager.Instance.ServerRequestPartyJoin(player.entityId, playerId);
        }
        
        [ServerRpc]
        public void JoinParty(string playerId)
        {
            PartyManager.Instance.ServerJoinParty(player.entityId, playerId);
        }
        [ServerRpc]
        public void LeaveParty()
        {
            if (Party == null) return;
            PartyManager.Instance.ServerLeaveParty(player.entityId, Party.LeaderId);
        }
    }
}