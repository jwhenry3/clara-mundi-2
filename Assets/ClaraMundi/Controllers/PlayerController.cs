using FishNet.Object;
using UnityEngine;

namespace ClaraMundi
{
    public class PlayerController : NetworkBehaviour
    {
        [HideInInspector]
        public Player player;

        protected virtual void Awake()
        {
            player = GetComponent<Player>() ?? GetComponentInParent<Player>();
        }
    }
}