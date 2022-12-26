using System;
using System.Collections.Generic;
using System.Linq;
using Backend.App;
using Unisave.Facades;
using Unisave.Serialization;
using UnityEngine;

namespace ClaraMundi
{
    public class PartyController : PlayerController
    {
        public List<string> PartyInvites = new();


        public event Action<PartyModel> PartyChanges;
        public event Action<List<string>> InviteChanges;

        public PartyModel Party;

        public override void OnStartClient()
        {
            base.OnStartClient();
            GetParty();
            GetInvitations();
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
        public async void GetInvitations()
        {
            PartyInvites = await OnFacet<PartyFacet>.CallAsync<List<string>>(
                nameof(PartyFacet.GetInvitations),
                player.Character.Name
            );
            InviteChanges?.Invoke(PartyInvites);
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
            UpdateParty(null);
        }

        public void MemberJoined(PartyMessage message)
        {
            if (Party == null) return;
            if (Party.Members.Contains(message.characterName)) return;
            Party.Members.Add(message.characterName);
            if (Party.Invitations.Contains(message.characterName))
                Party.Invitations.Remove(message.characterName);
            if (Party.Requests.Contains(message.characterName))
                Party.Requests.Remove(message.characterName);
            UpdateParty(Party);
            AlertManager.Instance.AddMessage(new AlertMessage()
            {
                Message = $"{message.characterName} joined the party."
            });
        }

        public void MemberLeft(PartyMessage message)
        {
            if (Party == null) return;
            if (Party.Members.Contains(message.characterName))
                Party.Members.Remove(message.characterName);
            UpdateParty(Party);
            AlertManager.Instance.AddMessage(new AlertMessage()
            {
                Message = $"{message.characterName} left the party."
            });
        }

        public void PlayerDeclinedInvite(PartyMessage message)
        {
            if (Party == null) return;
            if (Party.Invitations.Contains(message.characterName))
                Party.Invitations.Remove(message.characterName);
            if (Party.Requests.Contains(message.characterName))
                Party.Requests.Remove(message.characterName);
            UpdateParty(Party);
            AlertManager.Instance.AddMessage(new AlertMessage()
            {
                Message = $"{message.characterName} declined the party invite."
            });
        }

        public void PlayerCancelledRequest(PartyMessage message)
        {
            if (Party == null) return;
            if (Party.Invitations.Contains(message.characterName))
                Party.Invitations.Remove(message.characterName);
            if (Party.Requests.Contains(message.characterName))
                Party.Requests.Remove(message.characterName);
            UpdateParty(Party);
            AlertManager.Instance.AddMessage(new AlertMessage()
            {
                Message = $"{message.characterName} cancelled their join request."
            });
        }

        public void PlayerRequestedInvite(PartyMessage message)
        {
            if (Party == null) return;
            if (!Party.Requests.Contains(message.characterName))
                Party.Requests.Add(message.characterName);
            UpdateParty(Party);
            AlertManager.Instance.AddMessage(new AlertMessage()
            {
                Message = $"{message.characterName} wants to join the party."
            });
        }

        public void InvitedToParty(PartyMessage message)
        {
            if (!PartyInvites.Contains(message.characterName))
                PartyInvites.Add(message.characterName);
            InviteChanges?.Invoke(PartyInvites);
            AlertManager.Instance.AddMessage(new AlertMessage()
            {
                Message = $"{message.characterName} invited you to a party!"
            });
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

        public void LeaderChange(PartyMessage message)
        {
            Party.Leader = message.characterName;
            UpdateParty(Party);
            AlertManager.Instance.AddMessage(new AlertMessage()
            {
                Message = $"{message.characterName} is now the party leader."
            });
        }
    }
}