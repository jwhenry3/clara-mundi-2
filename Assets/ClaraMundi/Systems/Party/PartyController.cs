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

        public PartyModel Party;

        public override void OnStartClient()
        {
            base.OnStartClient();
            GetParty();
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

        public async void InviteToParty(string playerName)
        {
            await OnFacet<PartyFacet>.CallAsync<bool>(
                nameof(PartyFacet.InviteToParty),
                player.Character.Name,
                playerName
            );
        }

        public async void RequestJoin(string playerName)
        {
            await OnFacet<PartyFacet>.CallAsync<bool>(
                nameof(PartyFacet.JoinParty),
                player.Character.Name,
                playerName
            );
        }

        public async void AcceptRequest(string playerName)
        {
            await OnFacet<PartyFacet>.CallAsync<bool>(
                nameof(PartyFacet.InviteToParty),
                player.Character.Name,
                playerName
            );
        }

        public async void JoinParty(string playerName)
        {
            await OnFacet<PartyFacet>.CallAsync<bool>(
                nameof(PartyFacet.JoinParty),
                player.Character.Name,
                playerName
            );
        }

        public async void DeclineInvite(string playerName)
        {
            await OnFacet<PartyFacet>.CallAsync<bool>(
                nameof(PartyFacet.DeclineInvite),
                player.Character.Name,
                playerName
            );
        }

        public async void DeclineRequest(string playerName)
        {
            await OnFacet<PartyFacet>.CallAsync<bool>(
                nameof(PartyFacet.DeclineInvite),
                player.Character.Name,
                playerName
            );
        }

        public async void LeaveParty()
        {
            await OnFacet<PartyFacet>.CallAsync<bool>(
                nameof(PartyFacet.LeaveParty),
                player.Character.Name,
                false
            );
        }

        public async void GetParty()
        {
            Party = await OnFacet<PartyFacet>.CallAsync<PartyModel>(
                nameof(PartyFacet.GetParty),
                player.Character.Name
            );
            UpdateParty(Party);
        }

        public async void SendMessage(ChatMessage message)
        {
            if (!PartyClient.DoesPartyExist(Party)) return;
            var result = await OnFacet<ChatFacet>.CallAsync<bool>(
                nameof(ChatFacet.SendMessageToParty),
                new Backend.ChatMessage()
                {
                    message = message.Message,
                    senderName = message.SenderCharacterName
                }
            );
            if (!result)
            {
                ChatManager.ReceivedMessage(new ChatMessage()
                {
                    Type = ChatMessageType.System,
                    Channel = "System",
                    Message = "Could not send party message"
                });
            }
        }
        
        private void UpdateParty(PartyModel party)
        {
            Party = party;
            PartyChanges?.Invoke(party);
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

        public void AlreadyInParty(PartyMessage message)
        {
            if (string.IsNullOrEmpty(message.characterName))
            {
                AlertManager.Instance.AddMessage(new AlertMessage()
                {
                    Message = "Already in Party."
                });
                return;
            }
            AlertManager.Instance.AddMessage(new AlertMessage()
            {
                Message = $"{message.characterName} is already in a party."
            });
        }
    }
}