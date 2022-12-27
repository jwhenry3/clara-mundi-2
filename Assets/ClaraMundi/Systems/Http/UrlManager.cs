using System;
using UnityEngine;

namespace ClaraMundi
{
    [Serializable]
    public class UrlHolder
    {
        public string host = "localhost";
        public bool secure;
        public string httpPort = "3000";
        public string wsPort = "3000";

        public string Compose(bool forWebsocket = false)
        {
            var protocol = forWebsocket ? secure ? "wss" : "ws" : secure ? "https" : "http";
            if (forWebsocket)
                return protocol + "://" + host + ":" + wsPort;
            return protocol + "://" + host + ":" + httpPort;
        }
    }

    public class UrlManager : MonoBehaviour
    {
        public static UrlManager Instance;
        public UrlHolder MasterServerUrl;
        public UrlHolder LoginServerUrl;
        public UrlHolder ChatServerUrl;
        public UrlHolder QuestServerUrl;

        private void Awake()
        {
            Instance = this;
        }
    }
}