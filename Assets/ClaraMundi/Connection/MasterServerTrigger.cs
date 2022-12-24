using System;
using FishNet;
using FishNet.Transporting;
using UnityEngine;

namespace ClaraMundi
{
    public class MasterServerTrigger : MonoBehaviour
    {
        public GameObject NetworkManagerObject;
        private void Awake()
        {
            InstanceFinder.NetworkManager.ServerManager.OnServerConnectionState += OnServerConnectionState;
        }

        private void OnServerConnectionState(ServerConnectionStateArgs obj)
        {
            NetworkManagerObject.SetActive(obj.ConnectionState == LocalConnectionState.Started);
        }
    }
}