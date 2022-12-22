using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace ClaraMundi
{
    public class ConnectedPlayerManager : NetworkBehaviour
    {
        [SyncObject]
        public readonly SyncDictionary<string, CharacterModel> characterByName = new();

        public static ConnectedPlayerManager Instance;

        private void Awake()
        {
            Instance = this;
        }
    }
}