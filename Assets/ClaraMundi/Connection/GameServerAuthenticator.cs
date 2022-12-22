using System;
using System.Threading.Tasks;
using FishNet;
using FishNet.Authenticating;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Transporting;
using UnityEngine;

namespace ClaraMundi
{
    public struct GameServerRequest : IBroadcast
    {
        public string Token;
        public string Region;
        public string Name;
        public ushort Port;
        public int Capacity;
    }

    public struct GameServerResult : IBroadcast
    {
        public bool Allowed;
    }
    public class GameServerAuthenticator : Authenticator
    {
        public string MasterServerToken;
        public bool IsClient;

        private bool HasConnectedBefore;
        
        public override event Action<NetworkConnection, bool> OnAuthenticationResult;

        private void Awake()
        {
            if (!IsClient) return;
            base.InitializeOnce(GetComponent<NetworkManager>());
            
            NetworkManager.ClientManager.RegisterBroadcast<GameServerResult>(OnResult);
            NetworkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
            NetworkManager.ClientManager.StartConnection();
            HasConnectedBefore = true;
            Debug.Log(InstanceFinder.NetworkManager == NetworkManager);
        }

        public override void InitializeOnce(NetworkManager networkManager)
        {
            base.InitializeOnce(networkManager);
            if (IsClient) return;
            networkManager.ServerManager.OnRemoteConnectionState += ServerManager_OnRemoteConnectionState;
            //Listen for broadcast from client. Be sure to set requireAuthentication to false.
            networkManager.ServerManager.RegisterBroadcast<GameServerRequest>(OnAuthorized, false);
        }
        
        private void ServerManager_OnRemoteConnectionState(NetworkConnection conn, RemoteConnectionStateArgs args)
        {
            if (IsClient) return;
            if (args.ConnectionState != RemoteConnectionState.Stopped) return;
            if (MasterServerManager.Instance == null) return;
            if (!MasterServerManager.Instance.ServerNamesByClientId.ContainsKey(conn.ClientId)) return;
            var serverName = MasterServerManager.Instance.ServerNamesByClientId[conn.ClientId];
            var server = MasterServerManager.Instance.Servers[serverName];
            MasterServerManager.Instance.ServerUpdateStatus( conn, server.Name, server.Region,  server.Port, server.PlayerCapacity, false);
        }
        private async void ClientManager_OnClientConnectionState(ClientConnectionStateArgs args)
        {
            if (!IsClient) return;
            if (args.ConnectionState != LocalConnectionState.Started)
            {
                if (args.ConnectionState != LocalConnectionState.Stopped || !HasConnectedBefore) return;
                // wait 5 seconds before trying to reconnect
                await Task.Delay(5000);
                // Try reconnecting
                NetworkManager.ClientManager.StartConnection();
                return;
            }
            // let the master server know that the game server is starting
            var request = new GameServerRequest()
            {
                Name = Server.Instance.Name,
                Region = Server.Instance.Region,
                Token = MasterServerToken,
                Port = Server.Instance.Port,
                Capacity = Server.Instance.PlayerCapacity
            };
            NetworkManager.ClientManager.Broadcast(request);
        }

        public override void OnRemoteConnection(NetworkConnection connection)
        {
            Debug.Log("Received Connection");
        }

        void OnAuthorized(NetworkConnection conn, GameServerRequest request)
        {
            if (IsClient) return;
            if (MasterServerManager.Instance == null) return;
            var contains = MasterServerManager.Instance.Servers.ContainsKey(request.Name);
            var alreadyUp = false || contains && MasterServerManager.Instance.Servers[request.Name].Status;
            if (request.Token != MasterServerToken || alreadyUp)
            {
                var rb = new GameServerResult
                {
                    Allowed = false
                };
                NetworkManager.ServerManager.Broadcast(conn, rb, false);
                OnAuthenticationResult?.Invoke(conn, false);
                conn.Disconnect(true);
            }
            else
            {
                MasterServerManager.Instance.ServerUpdateStatus(conn, request.Name, request.Region, request.Port, request.Capacity,
                    true);
                var rb = new GameServerResult
                {
                    Allowed = true
                };
                NetworkManager.ServerManager.Broadcast(conn, rb, false);
                OnAuthenticationResult?.Invoke(conn, true);
            }
        }

        void OnResult(GameServerResult result)
        {
            if (!IsClient) return;
        }
    }
}