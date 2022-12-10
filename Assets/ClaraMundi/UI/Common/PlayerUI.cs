using System;
using UnityEngine;
namespace ClaraMundi
{
    public class PlayerUI : MonoBehaviour
    {
        protected Player player;
        protected Entity entity;
        public int OwnerId => GetOwnerId();

        Action<Player> Callback;

        private int GetOwnerId()
        {
            if (entity != null && entity.Owner != null) 
                return entity.Owner.ClientId;
            return -1;
        }
        public virtual void Start()
        {
            // ReSharper disable once Unity.NoNullPropagation
            Callback = OnPlayerChange;
            PlayerManager.Instance.OnPlayerChange += Callback;
            Callback(PlayerManager.Instance.LocalPlayer);

        }

        public virtual void OnDestroy()
        {
            PlayerManager.Instance.OnPlayerChange -= Callback;
        }

        protected virtual void OnPlayerChange(Player _player)
        {
            player = _player;
            entity = player ? player.Entity : null;
        }
    }
}