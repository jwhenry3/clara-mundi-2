using System;
using System.Threading.Tasks;
using Backend.App;
using FishNet;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Transporting.Tugboat;
using Unisave.Facades;
using UnityEngine;

namespace ClaraMundi
{
    public class GameServerManager : MonoBehaviour
    {
        public event Action<string> OnConnectToRegion;
        public event Action<string> OnChangeZone;

        private NetworkManager NetworkManager;

        private void Start()
        {
            NetworkManager = InstanceFinder.NetworkManager;
        }

        public async Task<GameServerEntity> GetServerForScene(string scene)
        {
            if (!RepoManager.Instance.RegionRepo.Zones.ContainsKey(scene)) return null;
            var zone = RepoManager.Instance.RegionRepo.Zones[scene];
            return await OnFacet<GameServerFacet>.CallAsync<GameServerEntity>(
                nameof(GameServerFacet.GetOnlineServerForRegion),
                zone.Region.Name
            );
        }

        public void DisconnectFromServer()
        {
            NetworkManager.ClientManager.StopConnection();
        }
        public void ConnectToServer(GameServerEntity server)
        {
            NetworkManager.ClientManager.StartConnection(server.Host, server.Port);
        }
    }
}