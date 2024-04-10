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
        public CharacterClass CurrentClass => Classes.ContainsKey(CurrentClassId.Value) ? Classes[CurrentClassId.Value]: new CharacterClass();
        public event Action OnStarted;
        public readonly SyncVar<string> entityName = new();

        public EntityType EntityType;
        public readonly SyncVar<string> entityId = new();

        public override void OnStartClient()
        {
            base.OnStartClient();
            OnStarted?.Invoke();
            EntityManager.Instance.Entities[entityId.Value] = this;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            OnStarted?.Invoke();
        }

    }
}