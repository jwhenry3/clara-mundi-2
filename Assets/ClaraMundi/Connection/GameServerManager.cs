using FishNet;
using FishNet.Managing;
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

        public ServerEntry GetServerForScene(string scene)
        {
            if (!RepoManager.Instance.RegionRepo.Zones.ContainsKey(scene)) return null;
            var zone = RepoManager.Instance.RegionRepo.Zones[scene];
            if (MasterServerApi.Instance.serversByRegion.ContainsKey(zone.Region.Name))
                return MasterServerApi.Instance.serversByRegion[zone.Region.Name][0];
            Debug.LogWarning("No server available for region");
            return null;
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