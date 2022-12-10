using System;
using FishNet.Object;

namespace ClaraMundi
{
    public class Entity : NetworkBehaviour
    {
        public event Action OnStarted;
        public string entityName = "";
        public string entityId = Guid.NewGuid().ToString();

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
    }
}