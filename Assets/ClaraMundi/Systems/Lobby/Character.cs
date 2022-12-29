namespace ClaraMundi
{
    public class Character
    {
        public string accountId;
        public string name;
        public string gender;
        public string race;
        public string area;
        public float position_x;
        public float position_y;
        public float position_z;
        public float rotation;

        public int level;
        public int exp;

        public int lastConnected;
        public int lastDisconnected;

        public bool hasConnectedBefore;
    }
}