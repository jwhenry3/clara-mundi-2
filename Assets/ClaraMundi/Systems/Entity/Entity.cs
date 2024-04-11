using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace ClaraMundi
{
  public class Entity : NetworkBehaviour
  {

    public readonly SyncVar<Character> Character = new();
    public readonly SyncDictionary<string, CharacterClass> Classes = new();

    public readonly SyncVar<string> CurrentClassId = new("adventurer");
    public CharacterClass CurrentClass => Classes.ContainsKey(CurrentClassId.Value) ? Classes[CurrentClassId.Value] : new CharacterClass();
    public event Action OnStarted;
    public readonly SyncVar<string> entityName = new();

    public EntityType EntityType;
    public readonly SyncVar<string> entityId = new();

    public override void OnStartClient()
    {
      base.OnStartClient();
      OnStarted?.Invoke();
      OnEntityId(entityId.Value, entityId.Value, false);
      entityId.OnChange += OnEntityId;
    }

    public override void OnStopClient()
    {
      base.OnStopClient();
      entityId.OnChange -= OnEntityId;
    }

    void OnEntityId(string prev, string next, bool asServer)
    {
      if (prev != null && EntityManager.Instance.Entities.ContainsKey(prev))
        EntityManager.Instance.Entities.Remove(prev);
      if (next != null)
        EntityManager.Instance.Entities[next] = this;
    }

    public override void OnStartServer()
    {
      base.OnStartServer();
      OnStarted?.Invoke();
    }

  }
}