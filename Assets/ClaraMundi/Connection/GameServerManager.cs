using System.Threading.Tasks;
using Backend.App;
using FishNet;
using FishNet.Managing;
using Unisave.Facades;
using UnityEngine;

namespace ClaraMundi
{
    public class GameServerManager : MonoBehaviour
    {

        private NetworkManager NetworkManager;

        public static GameServerManager Instance;

        private void Awake()
        {
            Instance = this;
        }

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