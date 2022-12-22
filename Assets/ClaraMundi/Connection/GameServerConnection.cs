using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using System;
using FishNet;
using FishNet.Managing.Scened;
using UnityEngine;
using UnityEngine.Serialization;

namespace ClaraMundi
{

    /// <summary>
    /// Spawns a player object for clients when they connect.
    /// Must be placed on or beneath the NetworkManager object.
    /// </summary>
    public class GameServerConnection : MonoBehaviour
    {
        #region Public.
        /// <summary>
        /// Called on the server when a player is spawned.
        /// </summary>
        public event Action<NetworkObject> OnSpawned;
        #endregion

        #region Serialized.
        /// <summary>
        /// Prefab to spawn for the game server.
        /// </summary>
        [Tooltip("Prefab to spawn for the game server.")]
        [SerializeField]
        private NetworkObject _gameServerPrefab;
        /// <summary>
        /// True to add player to the active scene when no global scenes are specified through the SceneManager.
        /// </summary>
        [Tooltip("True to add player to the active scene when no global scenes are specified through the SceneManager.")]
        [SerializeField]
        private bool _addToDefaultScene = true;
        #endregion

        #region Private.
        /// <summary>
        /// NetworkManager on this object or within this objects parents.
        /// </summary>
        private NetworkManager _networkManager;
        #endregion

        public static GameServerConnection Instance;

        private void Start()
        {
            InitializeOnce();
        }

        private void OnDestroy()
        {
            if (_networkManager != null)
                _networkManager.SceneManager.OnClientLoadedStartScenes -= SceneManager_OnClientLoadedStartScenes;
        }


        /// <summary>
        /// Initializes this script for use.
        /// </summary>
        private void InitializeOnce()
        {
            Instance = this;
            _networkManager = GetComponent<NetworkManager>();
            if (_networkManager == null)
            {
                Debug.LogWarning($"PlayerSpawner on {gameObject.name} cannot work as NetworkManager wasn't found on this object or within parent objects.");
                return;
            }

            _networkManager.SceneManager.OnClientLoadedStartScenes += SceneManager_OnClientLoadedStartScenes;
        }

        public void SpawnGameServerObject(NetworkConnection conn, bool asServer)
        {
            if (_gameServerPrefab == null)
            {
                Debug.LogWarning($"Game server prefab is empty and cannot be spawned for connection {conn.ClientId}.");
                return;
            }

            if (!MasterServerManager.Instance.ServerNamesByClientId.ContainsKey(conn.ClientId))
            {
                Debug.LogWarning($"Trying to spawn server object before authentication for connection {conn.ClientId}.");
                return;
            }
            Debug.Log("Spawn Server Object");
            NetworkObject nob = _networkManager.GetPooledInstantiated(_gameServerPrefab, true);
            nob.name = MasterServerManager.Instance.ServerNamesByClientId[conn.ClientId];
            _networkManager.ServerManager.Spawn(nob, conn);
            //If there are no global scenes 
            if (_addToDefaultScene)
                _networkManager.SceneManager.AddOwnerToDefaultScene(nob);

            OnSpawned?.Invoke(nob);
        }

        /// <summary>
        /// Called when a client loads initial scenes after connecting.
        /// </summary>
        private void SceneManager_OnClientLoadedStartScenes(NetworkConnection conn, bool asServer)
        {
            if (!asServer || !conn.Authenticated)
                return;
            SpawnGameServerObject(conn, asServer);
        }

    }


}