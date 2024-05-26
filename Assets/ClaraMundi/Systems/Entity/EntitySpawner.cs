using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

namespace ClaraMundi
{
  public enum SpawnerType
  {
    Player,
    NPC,
    Resource,
  }
  public class EntitySpawner : NetworkBehaviour
  {
    public SpawnerType Type;

    public GameObject ObjectToSpawn;

    public List<GameObject> Spawned = new();

    public override void OnStartServer()
    {
      base.OnStartServer();
      if (Type != SpawnerType.Player && ObjectToSpawn)
      {
        var instance = Instantiate(ObjectToSpawn);
        instance.transform.position = transform.position;
        instance.transform.rotation = transform.rotation;
        Spawned.Add(instance);
      }
    }
  }
}