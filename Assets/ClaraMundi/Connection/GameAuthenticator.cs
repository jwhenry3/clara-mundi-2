using System;
using System.Collections.Generic;
using FishNet.Authenticating;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Managing;
using UnityEngine;
using TMPro;
using FishNet.Transporting;

namespace ClaraMundi
{
    public struct PlayerNameBroadcast : IBroadcast
    {
        public string Name;
    }

    public struct PlayerNameResultBroadcast : IBroadcast
    {
        public bool Passed;
    }
    public class GameAuthenticator : Authenticator
    {
        public static string playerName;
        public static Dictionary<string, NetworkConnection> playerConnections = new();
        public static Dictionary<int, string> playerNamesByClientId = new();
        public override event Action<NetworkConnection, bool> OnAuthenticationResult;

        public override void InitializeOnce(NetworkManager networkManager)
        {
            base.InitializeOnce(networkManager);

            base.NetworkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
            //Listen for broadcast from client. Be sure to set requireAuthentication to false.
            base.NetworkManager.ServerManager.RegisterBroadcast<PlayerNameBroadcast>(OnPlayerName, false);
            //Listen to response from server.
            base.NetworkManager.ClientManager.RegisterBroadcast<PlayerNameResultBroadcast>(OnResult);
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
            SendPlayerName();
        }
        public override void OnRemoteConnection(NetworkConnection connection)
        {
            Debug.Log("Received Connection");
        }

        void OnPlayerName(NetworkConnection conn, PlayerNameBroadcast data)
        {
            bool authorized = !playerConnections.ContainsKey(data.Name.ToLower());
            if (authorized)
            {
                playerConnections[data.Name.ToLower()] = conn;
                playerNamesByClientId[conn.ClientId] = data.Name;
            }
            SendAuthenticationResponse(conn, authorized);
            OnAuthenticationResult?.Invoke(conn, authorized);
        }
        void OnResult(PlayerNameResultBroadcast data)
        {
            Debug.Log("Received Result: " + (data.Passed ? "Authenticated" : "Not Authenticated"));
        }

        private void SendAuthenticationResponse(NetworkConnection conn, bool authenticated)
        {
            /* Tell client if they authenticated or not. This is
            * entirely optional but does demonstrate that you can send
            * broadcasts to client on pass or fail. */
            PlayerNameResultBroadcast rb = new PlayerNameResultBroadcast()
            {
                Passed = authenticated
            };
            base.NetworkManager.ServerManager.Broadcast(conn, rb, false);
            if (!authenticated)
            {
                conn.Disconnect(true);
            }
        }

        public static void RemovePlayerReference(int ClientId, string PlayerName)
        {
            if (playerNamesByClientId.ContainsKey(ClientId))
                playerNamesByClientId.Remove(ClientId);
            if (playerConnections.ContainsKey(PlayerName.ToLower()))
                playerConnections.Remove(PlayerName.ToLower());
        }

        void SendPlayerName()
        {

            PlayerNameBroadcast pb = new PlayerNameBroadcast()
            {
                Name = playerName
            };

            base.NetworkManager.ClientManager.Broadcast(pb);
        }
    }
}