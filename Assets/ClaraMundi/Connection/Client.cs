using FishNet;
using FishNet.Managing;
using FishNet.Transporting;
using TMPro;
using Unisave.Facades;
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
            authenticationScreen.SetActive(false);
            characterScreen.SetActive(false);
            DontDestroyOnLoad(gameObject);
            networkManager = InstanceFinder.NetworkManager;
            CheckAuthentication();
        }

        private void CheckAuthentication()
        {
            OnFacet<PlayerFacet>
                .Call<PlayerEntity>(
                    nameof(PlayerFacet.GetPlayer)
                )
                .Then((player) =>
                {
                    Debug.Log("Received player");
                    AuthHolder.Player = player;
                    AuthHolder.Token = player != null ? player.token : "";
                    if (player != null)
                    {
                        authenticationScreen.SetActive(false);
                        characterScreen.SetActive(true);
                        // go to character screen
                    }
                    else
                    {
                        authenticationScreen.SetActive(true);
                        characterScreen.SetActive(false);
                    }
                }).Catch((e) =>
                {
                    authenticationScreen.SetActive(true);
                    characterScreen.SetActive(false);
                });

        }

        private void Start()
        {
            Physics.IgnoreLayerCollision(3, 3);
            networkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
        }

        void ClientManager_OnClientConnectionState(ClientConnectionStateArgs obj)
        {
            _clientState = obj.ConnectionState;
            joinLabel.text = GetNextStateText(_clientState);
        }


        private void OnDestroy()
        {
            networkManager.ClientManager.OnClientConnectionState -= ClientManager_OnClientConnectionState;
        }

        public void Connect()
        {
            networkManager.ClientManager.StartConnection();
        }

        public void OnButtonClick()
        {
            if (_clientState == LocalConnectionState.Stopped)
            {
                networkManager.ClientManager.StartConnection();
            }
            else
            {
                networkManager.ClientManager.StopConnection();
            }
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