using System;
using System.Collections.Generic;
using FishNet.Authenticating;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Transporting;
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

        public string ServerToken;

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
            characterNameByClientId.Remove(conn.ClientId);
            if (ConnectedPlayerManager.Instance.characterByName.ContainsKey(characterName))
                ConnectedPlayerManager.Instance.characterByName.Remove(characterName);
            if (connectionsByCharacterName.ContainsKey(characterName))
                connectionsByCharacterName.Remove(characterName);
            
            await LobbyApi.LogoutCharacter(ServerToken, characterName);
            
            if (MasterServerConnection.Instance != null)
                MasterServerConnection.Instance.UpdateServerList();
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
            if (characterNameByClientId.Count >= Server.Instance.PlayerCapacity)
            {
                SendAuthenticationResponse(conn, false);
                OnAuthenticationResult?.Invoke(conn, false);
                return;
            }

            data.CharacterName = data.CharacterName.ToLower();
            var result = await LobbyApi.VerifyCharacter(data.AccountToken, data.CharacterName);
            var character = result.character;
            bool authorized = character != null && !connectionsByCharacterName.ContainsKey(data.CharacterName);
            if (authorized)
            {
                connectionsByCharacterName[data.CharacterName] = conn;
                characterNameByClientId[conn.ClientId] = character.name;
                ConnectedPlayerManager.Instance.characterByName[character.name] = result.character;
                conn.OnLoadedStartScenes += OnLoadedStartScenes;
                if (MasterServerConnection.Instance != null)
                    MasterServerConnection.Instance.UpdateServerList();
            }

            SendAuthenticationResponse(conn, authorized);
            OnAuthenticationResult?.Invoke(conn, authorized);
        }

        private void OnLoadedStartScenes(NetworkConnection conn, bool asServer)
        {
            conn.OnLoadedStartScenes -= OnLoadedStartScenes;
            Debug.Log("Spawn Player!");
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
                conn.Disconnect(true);
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
                CharacterName = SessionManager.Instance.PlayerCharacter.name
            };

            NetworkManager.ClientManager.Broadcast(pb);
        }
    }
}