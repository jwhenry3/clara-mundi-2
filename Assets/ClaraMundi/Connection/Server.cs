using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Transporting;
using TMPro;
using UnityEngine;

namespace ClaraMundi
{
    public class Server : MonoBehaviour
    {
        [HideInInspector]
        public NetworkManager networkManager;
        public static Server Instance;

        public int PlayerCapacity = 100;

        public bool AutoStart;
        public string Region;
        public ushort Port = 7770;
        public string Name;
        public NetworkObject[] Singletons;

        private LocalConnectionState _serverState = LocalConnectionState.Stopped;

        private void Awake()
        {
            Instance = this;
            networkManager = InstanceFinder.NetworkManager;
        }

        private void Start()
        {
            Physics.IgnoreLayerCollision(3, 3);
            networkManager.ServerManager.OnServerConnectionState += ServerManager_OnServerConnectionState;
            if (AutoStart)
                networkManager.ServerManager.StartConnection(Port);
        }

        private void ServerManager_OnServerConnectionState(ServerConnectionStateArgs args)
        {
            _serverState = args.ConnectionState;
            if (args.ConnectionState != LocalConnectionState.Started) return;
            if (!networkManager.IsServer) return;
            foreach (var singleton in Singletons)
            {
                NetworkObject nob = networkManager.GetPooledInstantiated(singleton, true);
                networkManager.ServerManager.Spawn(nob);
            }

            // Perhaps this is not a game server, but a master server
            if (string.IsNullOrEmpty(Region)) return;
            if (!RepoManager.Instance.RegionRepo.Regions.ContainsKey(Region)) return;
            
            var region = RepoManager.Instance.RegionRepo.Regions[Region];
            var keys = new List<Zone>(region.Zones).Select(zone => zone.Key);
            SceneLoadData sld = new SceneLoadData(keys.ToArray());
            // load all zones for the region specified
            networkManager.SceneManager.LoadConnectionScenes(sld);
        }

        private void OnDestroy()
        {
            networkManager.ServerManager.OnServerConnectionState -= ServerManager_OnServerConnectionState;
        }

        private static string GetNextStateText(LocalConnectionState state)
        {
            return state switch
            {
                LocalConnectionState.Stopped => "Host",
                LocalConnectionState.Starting => "Starting Host",
                LocalConnectionState.Stopping => "Stopping Host",
                LocalConnectionState.Started => "Stop Hosting",
                _ => "Invalid"
            };
        }
    }
}