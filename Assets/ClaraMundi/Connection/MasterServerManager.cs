using System;
using System.Collections.Generic;
using Backend.App;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Sirenix.OdinInspector;
using Unisave.Facades;
using UnityEngine;

namespace ClaraMundi
{
    [Serializable]
    public class GameServer
    {
        public string Name;
        public string Host;
        public string Region;
        public bool Status;
        public ushort Port;
        public int PlayerCount;
        public int PlayerCapacity;
    }

    public class MasterServerManager : NetworkBehaviour
    {
        [ShowInInspector] [SyncObject] public readonly SyncDictionary<string, GameServer> Servers = new();

        [ShowInInspector] public readonly Dictionary<int, string> ServerNamesByClientId = new();

        public static MasterServerManager Instance;

        private void Awake()
        {
            Instance = this;
            Debug.Log("Spawned!");
        }

        public override async void OnStartServer()
        {
            base.OnStartServer();
            await OnFacet<GameServerFacet>.CallAsync(
                nameof(GameServerFacet.ResetServers)
            );
        }

        [ServerRpc(RequireOwnership = false)]
        public void UpdatePlayerCount(string serverName, int playerCount)
        {
            Servers[serverName] = new GameServer()
            {
                Name = serverName,
                Host = Servers[serverName].Host,
                Region = Servers[serverName].Region,
                Status = true,
                PlayerCount = playerCount
            };
        }

        public async void ServerUpdateStatus(NetworkConnection conn, string serverName, string region, ushort port,
            int capacity,
            bool status)
        {
            var contains = Servers.ContainsKey(serverName);
            Servers[serverName] = new GameServer()
            {
                Name = serverName,
                Host = conn.GetAddress(),
                Region = region,
                Status = status,
                Port = port,
                PlayerCount = !status || !contains ? 0 : Servers[serverName].PlayerCount,
                PlayerCapacity = capacity
            };
            if (status)
                ServerNamesByClientId[conn.ClientId] = serverName;
            else if (ServerNamesByClientId.ContainsKey(conn.ClientId))
                ServerNamesByClientId.Remove(conn.ClientId);
            await OnFacet<GameServerFacet>.CallAsync<bool>(
                nameof(GameServerFacet.SaveServer),
                GameServerAuthenticator.Instance.MasterServerToken,
                new GameServerModel()
                {
                    Name = serverName,
                    Host = conn.GetAddress(),
                    Region = region,
                    Status = status,
                    Port = port,
                    PlayerCount = Servers[serverName].PlayerCount,
                    PlayerCapacity = capacity
                }
            );
        }
    }
}