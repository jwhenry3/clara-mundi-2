using FishNet;
using FishNet.Managing;
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

        public TextMeshProUGUI hostLabel;

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
        }

        private void ServerManager_OnServerConnectionState(ServerConnectionStateArgs obj)
        {
            _serverState = obj.ConnectionState;
            hostLabel.text = GetNextStateText(_serverState);
        }

        private void OnDestroy()
        {
            networkManager.ServerManager.OnServerConnectionState -= ServerManager_OnServerConnectionState;
        }

        public void OnButtonClick()
        {
            if (_serverState == LocalConnectionState.Stopped)
                networkManager.ServerManager.StartConnection();
            else
                networkManager.ServerManager.StopConnection(true);
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