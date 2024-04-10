﻿using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace ClaraMundi
{
    public class ConnectedPlayerManager : NetworkBehaviour
    {
        public readonly SyncDictionary<string, Character> characterByName = new();

        public static ConnectedPlayerManager Instance;

        private void Awake()
        {
            Instance = this;
            
        }
    }
}