using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace ClaraMundi
{
    public class Entity : NetworkBehaviour
    {
        [SyncVar]
        public Character Character;
        [SyncObject]
        public readonly SyncDictionary<string, CharacterClass> Classes = new();

        [SyncVar] public string CurrentClassId = "adventurer";
        public CharacterClass CurrentClass => Classes.ContainsKey(CurrentClassId) ? Classes[CurrentClassId]: new CharacterClass();
        public event Action OnStarted;
        public event Action<string> NameChange;
        [SyncVar(OnChange = nameof(OnNameChange))]
        public string entityName = "";

        public EntityType EntityType;
        [SyncVar]
        public string entityId;

        public override void OnStartClient()
        {
            base.OnStartClient();
            OnStarted?.Invoke();
            EntityManager.Instance.Entities[entityId] = this;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            OnStarted?.Invoke();
        }
        void OnNameChange(string oldValue, string newValue, bool asServer)
        {
            NameChange?.Invoke(newValue);
        }

    }
}