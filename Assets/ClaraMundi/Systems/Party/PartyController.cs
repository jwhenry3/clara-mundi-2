using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Object.Synchronizing.SecretMenu;

namespace ClaraMundi
{
    using Responder = Func<object, object>;

    public class PartyController : PlayerController
    {
        [SyncObject(ReadPermissions =  ReadPermission.OwnerOnly)]
        public readonly SyncList<string> PartyInvites = new();

        private RequestResponse requestResponse;
        public event Action<Party> PartyChanges;
        public event Action<SyncList<string>> InviteChanges;

        [SyncVar(OnChange = nameof(OnPartyChange))]
        public Party Party;

        [SyncVar(OnChange = nameof(OnMessage))]
        public ChatMessage LastMessage;

        protected override void Awake()
        {
            base.Awake();
            requestResponse = GetComponent<RequestResponse>();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            requestResponse.Responders.Add(nameof(ServerIsInParty), ServerIsInParty);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (IsOwner)
                PartyInvites.OnChange += OnInviteChanges;
        }

        private void OnDestroy()
        {
            if (IsClient && IsOwner)
                PartyInvites.OnChange -= OnInviteChanges;
        }

        private void OnInviteChanges(SyncListOperation op, int index, string previous, string next, bool asServer)
        {
            InviteChanges?.Invoke(PartyInvites);
        }

        [ServerRpc]
        public void CreateParty()
        {
            PartyManager.Instance.CreateParty(player.Character.name);
        }

        [ServerRpc]
        public void DisbandParty()
        {
            PartyManager.Instance.DisbandParty(player.Character.name);
        }

        [ServerRpc]
        public void InviteToParty(string playerName)
        {
            PartyManager.Instance.InviteToParty(player.Character.name, playerName);
        }

        [ServerRpc]
        public void RequestJoin(string playerName)
        {
            PartyManager.Instance.JoinParty(player.Character.name, playerName);
        }

        [ServerRpc]
        public void AcceptRequest(string playerName)
        {
            PartyManager.Instance.AcceptRequest(player.Character.name, playerName);
        }

        [ServerRpc]
        public void JoinParty(string playerName)
        {
            PartyManager.Instance.JoinParty(player.Character.name, playerName);
        }

        [ServerRpc]
        public void DeclineInvite(string playerName)
        {
            PartyManager.Instance.DeclineInvite(player.Character.name, playerName);
        }

        [ServerRpc]
        public void DeclineRequest(string playerName)
        {
            PartyManager.Instance.DeclineRequest(player.Character.name, playerName);
        }

        [ServerRpc]
        public void LeaveParty()
        {
            PartyManager.Instance.LeaveParty(player.Character.name);
        }

        [ServerRpc]
        public void SendChatMessage(ChatMessage message)
        {
            PartyManager.Instance.SendChatMessage(message);
        }

        private void OnMessage(ChatMessage previous, ChatMessage next, bool asServer)
        {
            if (asServer) return;
            if (previous?.MessageId == next?.MessageId) return;
            ChatManager.ReceivedMessage(next);
        }

        public async Task<bool> IsInParty(string playerName)
        {
            return (await requestResponse.Request<string, bool>(nameof(ServerIsInParty), playerName));
        }

        protected object ServerIsInParty(object playerName)
        {
            if (playerName is string p)
                return PartyManager.Instance.Parties.ContainsKey(p);
            return true;
        }

        private void UpdateParty(Party party)
        {
            Party = party;
            PartyChanges?.Invoke(party);
        }

        public void ClearParty()
        {
            UpdateParty(null);
        }

        [TargetRpc]
        public void CreatedParty(NetworkConnection conn)
        {
            ChatManager.ReceivedMessage(new ChatMessage()
            {
                Type = ChatMessageType.System,
                Message = $"Created the party successfully."
            });
        }
        [TargetRpc]
        public void MemberJoined(NetworkConnection conn, string _player)
        {
            ChatManager.ReceivedMessage(new ChatMessage()
            {
                Type = ChatMessageType.System,
                Message = $"{_player} joined the party."
            });
        }

        [TargetRpc]
        public void MemberLeft(NetworkConnection conn, string _player)
        {
            ChatManager.ReceivedMessage(new ChatMessage()
            {
                Type = ChatMessageType.System,
                Message =  $"{_player} left the party."
            });
        }

        [TargetRpc]
        public void YouLeft(NetworkConnection conn)
        {
            ChatManager.ReceivedMessage(new ChatMessage()
            {
                Type = ChatMessageType.System,
                Message = $"You left the party."
            });
        }

        [TargetRpc]
        public void DisbandedParty(NetworkConnection conn)
        {
            ChatManager.ReceivedMessage(new ChatMessage()
            {
                Type = ChatMessageType.System,
                Message =  $"The party disbanded."
            });
        }

        [TargetRpc]
        public void PlayerDeclinedInvite(NetworkConnection conn, string _player)
        {
            ChatManager.ReceivedMessage(new ChatMessage()
            {
                Type = ChatMessageType.System,
                Message =  $"{_player} declined the party invite."
            });
        }

        [TargetRpc]
        public void PlayerCancelledRequest(NetworkConnection conn, string _player)
        {
            ChatManager.ReceivedMessage(new ChatMessage()
            {
                Type = ChatMessageType.System,
                Message =  $"{_player} cancelled their join request."
            });
        }

        [TargetRpc]
        public void PlayerRequestedInvite(NetworkConnection conn, string _player)
        {
            ChatManager.ReceivedMessage(new ChatMessage()
            {
                Type = ChatMessageType.System,
                Message =   $"{_player} wants to join the party."
            });
        }

        [TargetRpc]
        public void InvitedToParty(NetworkConnection conn, string _player)
        {
            ChatManager.ReceivedMessage(new ChatMessage()
            {
                Type = ChatMessageType.System,
                Message =   $"{_player} invited you to a party!"
            });
        }

        [TargetRpc]
        public void PlayerInvited(NetworkConnection conn, string _player)
        {
            ChatManager.ReceivedMessage(new ChatMessage()
            {
                Type = ChatMessageType.System,
                Message =   $"Invited {_player} Successfully!"
            });
        }

        [TargetRpc]
        public void PartyFull(NetworkConnection conn)
        {
            ChatManager.ReceivedMessage(new ChatMessage()
            {
                Type = ChatMessageType.System,
                Message =   "Cannot join the party. The party is full."
            });
        }

        [TargetRpc]
        public void AlreadyInParty(NetworkConnection conn, string _player)
        {
            if (string.IsNullOrEmpty(_player))
            {
                ChatManager.ReceivedMessage(new ChatMessage()
                {
                    Type = ChatMessageType.System,
                    Message =   "Already in Party."
                });
                return;
            }

            ChatManager.ReceivedMessage(new ChatMessage()
            {
                Type = ChatMessageType.System,
                Message =  $"{_player} is already in a party."
            });
        }

        [TargetRpc]
        public void LeaderChange(NetworkConnection conn, string _player)
        {
            ChatManager.ReceivedMessage(new ChatMessage()
            {
                Type = ChatMessageType.System,
                Message =  $"{_player} is now the party leader."
            });
        }

        private void OnPartyChange(Party previous, Party next, bool asServer)
        {
            if (asServer) return;
            PartyChanges?.Invoke(next);
        }
    }
}