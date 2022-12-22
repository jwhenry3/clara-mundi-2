using System;
using System.Collections.Generic;
using Backend.App;
using FishNet.Authenticating;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using Unisave.Facades;
using UnityEngine;

namespace ClaraMundi
{
    public struct CharacterSelectionBroadcast : IBroadcast
    {
        public string AccountToken;
        public string CharacterName;
    }

    public struct CharacterSelectionResult : IBroadcast
    {
        public bool Passed;
    }

    public class GameAuthenticator : Authenticator
    {
        public static readonly Dictionary<string, NetworkConnection> connectionsByCharacterName = new();
        public static readonly Dictionary<int, string> characterNameByClientId = new();
        public override event Action<NetworkConnection, bool> OnAuthenticationResult;

        public override void InitializeOnce(NetworkManager networkManager)
        {
            base.InitializeOnce(networkManager);

            NetworkManager.ServerManager.OnRemoteConnectionState += ServerManager_OnRemoteConnectionState;
            NetworkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
            //Listen for broadcast from client. Be sure to set requireAuthentication to false.
            NetworkManager.ServerManager.RegisterBroadcast<CharacterSelectionBroadcast>(OnAuthorized, false);
            //Listen to response from server.
            NetworkManager.ClientManager.RegisterBroadcast<CharacterSelectionResult>(OnResult);
        }

        private async void ServerManager_OnRemoteConnectionState(NetworkConnection conn, RemoteConnectionStateArgs args)
        {
            if (args.ConnectionState != RemoteConnectionState.Stopped) return;
            if (!characterNameByClientId.ContainsKey(conn.ClientId)) return;
            var characterName = characterNameByClientId[conn.ClientId];
            if (!ConnectedPlayerManager.Instance.characterByName.ContainsKey(characterName)) return;
            var character = ConnectedPlayerManager.Instance.characterByName[characterName];
            await OnFacet<CharacterFacet>.CallAsync<CharacterEntity>(
                nameof(CharacterFacet.ServerCharacterLeavingGameServer),
                "", // server token
                character.CharacterId
            );
            ConnectedPlayerManager.Instance.characterByName.Remove(character.Name);
            connectionsByCharacterName.Remove(character.Name);
            characterNameByClientId.Remove(conn.ClientId);
        }
        private void ClientManager_OnClientConnectionState(ClientConnectionStateArgs args)
        {
            /* If anything but the started state then exit early.
             * Only try to authenticate on started state. The server
            * doesn't have to send an authentication request before client
            * can authenticate, that is entirely optional and up to you. In this
            * example the client tries to authenticate soon as they connect. */
            if (args.ConnectionState != LocalConnectionState.Started)
                return;
            SendSelectedCharacter();
        }

        public override void OnRemoteConnection(NetworkConnection connection)
        {
            Debug.Log("Received Connection");
        }

        async void OnAuthorized(NetworkConnection conn, CharacterSelectionBroadcast data)
        {
            data.CharacterName = data.CharacterName.ToLower();
            var character = await OnFacet<CharacterFacet>.CallAsync<CharacterEntity>(
                nameof(CharacterFacet.ServerCharacterJoiningGameServer),
                "", // server token
                data.AccountToken,
                data.CharacterName
            );
            bool authorized = character != null && !connectionsByCharacterName.ContainsKey(data.CharacterName);
            if (authorized)
            {
                var model = new CharacterModel()
                {
                    CharacterId = character.EntityId,
                    Name = character.Name,
                    Gender = character.Gender,
                    Race = character.Race,
                    Position = character.Position,
                    Rotation = character.Rotation,
                    Level = character.Level,
                    TotalExp = character.TotalExp
                };
                connectionsByCharacterName[data.CharacterName] = conn;
                characterNameByClientId[conn.ClientId] = character.Name;
                ConnectedPlayerManager.Instance.characterByName[character.Name] = model;
                conn.OnLoadedStartScenes += OnLoadedStartScenes;
            }

            SendAuthenticationResponse(conn, authorized);
            OnAuthenticationResult?.Invoke(conn, authorized);
        }

        private void OnLoadedStartScenes(NetworkConnection conn, bool asServer)
        {
            conn.OnLoadedStartScenes -= OnLoadedStartScenes;
            PlayerConnection.Instance.SpawnPlayer(conn, asServer);
        }

        void OnResult(CharacterSelectionResult data)
        {
            Debug.Log("Received Result: " + (data.Passed ? "Authenticated" : "Not Authenticated"));
        }

        private void SendAuthenticationResponse(NetworkConnection conn, bool authenticated)
        {
            /* Tell client if they authenticated or not. This is
            * entirely optional but does demonstrate that you can send
            * broadcasts to client on pass or fail. */
            var rb = new CharacterSelectionResult
            {
                Passed = authenticated
            };
            NetworkManager.ServerManager.Broadcast(conn, rb, false);
            if (!authenticated)
            {
                conn.Disconnect(true);
            }
        }

        public static void RemovePlayerReference(int ClientId, string PlayerName)
        {
            if (characterNameByClientId.ContainsKey(ClientId))
                characterNameByClientId.Remove(ClientId);
            if (connectionsByCharacterName.ContainsKey(PlayerName.ToLower()))
                connectionsByCharacterName.Remove(PlayerName.ToLower());
            if (ConnectedPlayerManager.Instance.characterByName.ContainsKey(PlayerName))
                ConnectedPlayerManager.Instance.characterByName.Remove(PlayerName);
        }

        private void SendSelectedCharacter()
        {
            if (SessionManager.Instance.PlayerAccount == null) return;
            if (SessionManager.Instance.PlayerCharacter == null) return;
            var pb = new CharacterSelectionBroadcast
            {
                AccountToken = SessionManager.Instance.PlayerAccount.token,
                CharacterName = SessionManager.Instance.PlayerCharacter.Name
            };

            NetworkManager.ClientManager.Broadcast(pb);
        }
    }
}