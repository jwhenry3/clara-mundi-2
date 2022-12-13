using FishNet;
using FishNet.Managing;
using FishNet.Transporting;
using TMPro;
using UnityEngine;

namespace ClaraMundi
{
    public class Client : MonoBehaviour
    {
        [HideInInspector]
        public NetworkManager networkManager;
        public static Client Instance;
        public TextMeshProUGUI joinLabel;

        public GameObject authenticationScreen;
        public GameObject characterScreen;

        private LocalConnectionState _clientState = LocalConnectionState.Stopped;

        private void Awake()
        {
            Instance = this;
            // authenticationScreen.SetActive(false);
            // characterScreen.SetActive(false);
            networkManager = InstanceFinder.NetworkManager;
            CheckAuthentication();
        }

        private void CheckAuthentication()
        {

        }

        private void Start()
        {
            Physics.IgnoreLayerCollision(3, 3);
            networkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
        }

        void ClientManager_OnClientConnectionState(ClientConnectionStateArgs obj)
        {
            _clientState = obj.ConnectionState;
            // joinLabel.text = GetNextStateText(_clientState);
            if (obj.ConnectionState == LocalConnectionState.Stopped)
                ChatWindowUI.Instance.ClearMessages();
        }


        private void OnDestroy()
        {
            networkManager.ClientManager.OnClientConnectionState -= ClientManager_OnClientConnectionState;
        }

        private static string GetNextStateText(LocalConnectionState state)
        {
            return state switch
            {
                LocalConnectionState.Stopped => "Join",
                LocalConnectionState.Starting => "Starting Client",
                LocalConnectionState.Stopping => "Stopping Client",
                LocalConnectionState.Started => "Leave Server",
                _ => "Invalid"
            };
        }
    }
}