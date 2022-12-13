using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace ClaraMundi
{
    public class Entity : NetworkBehaviour
    {
        public event Action OnStarted;
        public event Action<string> NameChange;
        [SyncVar(OnChange = "OnNameChange")]
        public string entityName = "";
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
            entityId = Guid.NewGuid().ToString();
            OnStarted?.Invoke();
        }
        void OnNameChange(string oldValue, string newValue, bool asServer)
        {
            NameChange?.Invoke(newValue);
        }
    }
}