using System;
using System.Collections.Generic;
using Backend.App;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Unisave.Facades;
using UnityEngine;

namespace ClaraMundi
{
    public class PartyController : PlayerController
    {
        public readonly List<string> PartyInvites = new();


        public event Action<PartyModel> PartyChanges;
        public event Action<List<string>> InviteChanges;

        [SyncVar(OnChange = nameof(Client_OnChange))]
        public PartyModel Party;

        private void Client_OnChange(PartyModel lastParty, PartyModel nextParty, bool asServer)
        {
            PartyChanges?.Invoke(nextParty);
        }

        public async void CreateParty()
        {
            var result = await OnFacet<PartyFacet>.CallAsync<bool>(
                nameof(PartyFacet.CreateParty),
                player.Character.Name
            );
            if (!result)
                Debug.LogWarning("Could not create a party");
        }

        public async void DisbandParty()
        {
            await OnFacet<PartyFacet>.CallAsync<bool>(
                nameof(PartyFacet.LeaveParty),
                player.Character.Name,
                true
            );
        }

        public async void InviteToParty(string playerId)
        {
            await OnFacet<PartyFacet>.CallAsync<bool>(
                nameof(PartyFacet.InviteToParty),
                player.Character.Name,
                playerId
            );
        }

        public async void RequestJoin(string playerId)
        {
            await OnFacet<PartyFacet>.CallAsync<bool>(
                nameof(PartyFacet.JoinParty),
                player.Character.Name,
                playerId
            );
        }

        public async void AcceptRequest(string playerId)
        {
            await OnFacet<PartyFacet>.CallAsync<bool>(
                nameof(PartyFacet.InviteToParty),
                player.Character.Name,
                playerId
            );
        }

        public async void JoinParty(string playerId)
        {
            await OnFacet<PartyFacet>.CallAsync<bool>(
                nameof(PartyFacet.JoinParty),
                player.Character.Name,
                playerId
            );
        }

        public async void DeclineInvite(string playerId)
        {
            await OnFacet<PartyFacet>.CallAsync<bool>(
                nameof(PartyFacet.DeclineInvite),
                player.Character.Name,
                playerId
            );
        }

        public async void DeclineRequest(string playerId)
        {
            await OnFacet<PartyFacet>.CallAsync<bool>(
                nameof(PartyFacet.DeclineInvite),
                player.Character.Name,
                playerId
            );
        }

        public async void LeaveParty()
        {
            await OnFacet<PartyFacet>.CallAsync<bool>(
                nameof(PartyFacet.LeaveParty),
                player.Character.Name
            );
        }

        public async void GetParty()
        {
            UpdateParty(await OnFacet<PartyFacet>.CallAsync<PartyModel>(
                nameof(PartyFacet.GetParty),
                player.Character.Name
            ));
        }
        
        [ServerRpc]
        private void UpdateParty(PartyModel party)
        {
            Party = party;
        }
        
        public void ClearParty()
        {
            if (Party == null) return;
            UpdateParty(null);
        }

        public void MemberJoined(PartyMessage message)
        {
            if (Party.Members.Contains(message.characterName)) return;
            Party.Members.Add(message.characterName);
            UpdateParty(Party);
        }

        public void MemberLeft(PartyMessage message)
        {
            if (!Party.Members.Contains(message.characterName)) return;
            Party.Members.Remove(message.characterName);
            UpdateParty(Party);
        }

        public void PlayerDeclinedInvite(PartyMessage message)
        {
            if (!Party.Invitations.Contains(message.characterName)) return;
            Party.Invitations.Remove(message.characterName);
            UpdateParty(Party);
        }

        public void PlayerCancelledRequest(PartyMessage message)
        {
            if (!Party.Requests.Contains(message.characterName)) return;
            Party.Requests.Remove(message.characterName);
            UpdateParty(Party);
        }

        public void PlayerRequestedInvite(PartyMessage message)
        {
            if (Party.Requests.Contains(message.characterName)) return;
            Party.Requests.Add(message.characterName);
            UpdateParty(Party);
        }

        public void InvitedToParty(PartyMessage message)
        {
            if (PartyInvites.Contains(message.characterName)) return;
            PartyInvites.Add(message.characterName);
            InviteChanges?.Invoke(PartyInvites);
        }

        public void PlayerInvited(PartyMessage message)
        {
            AlertManager.Instance.AddMessage(new AlertMessage()
            {
                Message = $"Invited {message.characterName} Successfully!"
            });
        }

        public void PartyFull()
        {
            AlertManager.Instance.AddMessage(new AlertMessage()
            {
                Message = "Cannot join the party. The party is full."
            });
        }
    }
}