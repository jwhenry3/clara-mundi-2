using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace ClaraMundi
{
  public class Entity : NetworkBehaviour
  {

    public string ServerEntityId;
    public string ServerEntityName;

    public readonly SyncVar<Character> Character = new();
    public readonly SyncDictionary<string, CharacterClass> Classes = new();

    public readonly SyncVar<string> CurrentClassId = new("adventurer");
    public CharacterClass CurrentClass => Classes.ContainsKey(CurrentClassId.Value) ? Classes[CurrentClassId.Value] : new CharacterClass();
    public event Action OnStarted;
    public readonly SyncVar<string> entityName = new();

    public EntityType EntityType;
    public readonly SyncVar<string> entityId = new();


    public override void OnStartNetwork()
    {
      base.OnStartNetwork();
      if (IsServerStarted)
      {
        if (string.IsNullOrEmpty(entityId.Value))
          entityId.Value = ServerEntityId;
        if (string.IsNullOrEmpty(entityName.Value))
          entityName.Value = ServerEntityName;
      }
      OnEntityId(entityId.Value, entityId.Value, false);
      entityId.OnChange += OnEntityId;
      entityName.OnChange += OnEntityName;
      OnStarted?.Invoke();
    }
    public override void OnStopNetwork()
    {
      base.OnStopNetwork();
      entityId.OnChange -= OnEntityId;
      entityName.OnChange -= OnEntityName;
    }

    void OnEntityId(string prev, string next, bool asServer)
    {
      EntityManager.Instance.ChangeId(prev, next, this);
    }
    void OnEntityName(string prev, string next, bool asServer)
    {
      EntityManager.Instance.ChangeName(prev, next, this);
    }

  }
}