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