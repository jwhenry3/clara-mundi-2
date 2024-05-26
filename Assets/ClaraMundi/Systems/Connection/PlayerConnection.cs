using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using System;
using FishNet;
using UnityEngine;
using UnityEngine.Serialization;

namespace ClaraMundi
{

  /// <summary>
  /// Spawns a player object for clients when they connect.
  /// Must be placed on or beneath the NetworkManager object.
  /// </summary>
  public class PlayerConnection : MonoBehaviour
  {
    #region Public.
    /// <summary>
    /// Called on the server when a player is spawned.
    /// </summary>
    public event Action<NetworkObject> OnSpawned;
    #endregion

    #region Serialized.
    /// <summary>
    /// Prefab to spawn for the player.
    /// </summary>
    [Tooltip("Prefab to spawn for the player.")]
    [SerializeField]
    private NetworkObject _playerPrefab;
    /// <summary>
    /// True to add player to the active scene when no global scenes are specified through the SceneManager.
    /// </summary>
    [Tooltip("True to add player to the active scene when no global scenes are specified through the SceneManager.")]
    [SerializeField]
    private bool _addToDefaultScene = true;
    /// <summary>
    /// Areas in which players may spawn.
    /// </summary>
    [Tooltip("Areas in which players may spawn.")]
    [FormerlySerializedAs("_spawns")]
    public Transform[] Spawns = new Transform[0];
    #endregion

    #region Private.
    /// <summary>
    /// NetworkManager on this object or within this objects parents.
    /// </summary>
    private NetworkManager _networkManager;
    /// <summary>
    /// Next spawns to use.
    /// </summary>
    private int _nextSpawn;
    #endregion

    public static PlayerConnection Instance;

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
      _networkManager = InstanceFinder.NetworkManager;
      if (_networkManager == null)
      {
        Debug.LogWarning($"PlayerSpawner on {gameObject.name} cannot work as NetworkManager wasn't found on this object or within parent objects.");
        return;
      }

      _networkManager.SceneManager.OnClientLoadedStartScenes += SceneManager_OnClientLoadedStartScenes;
    }

    public void SpawnPlayer(NetworkConnection conn, bool asServer)
    {
      if (_playerPrefab == null)
      {
        Debug.LogWarning($"Player prefab is empty and cannot be spawned for connection {conn.ClientId}.");
        return;
      }

      if (!GameAuthenticator.characterNameByClientId.ContainsKey(conn.ClientId))
      {
        Debug.LogWarning($"Trying to spawn player before authentication for connection {conn.ClientId}.");
        return;
      }

      var characterName = GameAuthenticator.characterNameByClientId[conn.ClientId];
      var character = ConnectedPlayerManager.Instance.characterByName[characterName];


      NetworkObject nob = _networkManager.GetPooledInstantiated(_playerPrefab, true);
      character.name = character.name.ToLower();
      var player = nob.GetComponent<Player>();
      foreach (var characterClass in character.characterClasses)
      {
        player.Entity.Classes.Add(characterClass.classId, characterClass);
        if (characterClass.isCurrent)
          player.ServerChangeClass(characterClass.classId);
      }
      // reset list so it does not get synced to all clients
      player.Entity.Character.Value = character;
      player.Stats.player = player;

      player.Entity.entityName.Value = character.name.ToLower();
      player.Entity.entityId.Value = character.accountId;
      player.Stats.ComputeStats();

      var rotation = Quaternion.identity;
      rotation.y = character.rotation;
      var position = new Vector3(character.position_x, character.position_y, character.position_z);
      nob.transform.SetPositionAndRotation(position, rotation);
      _networkManager.ServerManager.Spawn(nob, conn);

      _networkManager.SceneManager.AddOwnerToDefaultScene(nob);

      OnSpawned?.Invoke(nob);
    }

    /// <summary>
    /// Called when a client loads initial scenes after connecting.
    /// </summary>
    private void SceneManager_OnClientLoadedStartScenes(NetworkConnection conn, bool asServer)
    {
      if (!asServer)
        return;
      Debug.Log("Spawn Player ");
    }

  }


}