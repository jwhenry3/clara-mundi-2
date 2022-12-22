
    using System;
    using Unisave.Entities;

    public class GameServerEntity : Entity
    {
        public string Name;
        public string Region;
        public string Host;
        public ushort Port;
        public bool Status;
        
        public int PlayerCount;
        public int PlayerCapacity;
    }

    [Serializable]
    public class GameServerModel
    {
        public string Name;
        public string Region;
        public string Host;
        public ushort Port;
        public bool Status;

        public int PlayerCount;
        public int PlayerCapacity;
    }