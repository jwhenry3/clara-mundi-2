using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ClaraMundi
{
    public class ServerEntry
    {
        public string label;
        public string host;
        public ushort port;
        public bool status;
        public int playerCapacity;
        public int currentPlayers;
    }

    public class MasterServerConnection : WebSocketConnection
    {
        public static MasterServerConnection Instance;
        private float syncInterval = 30f;
        private float currentTick = 0;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            UpdateServerUrl(UrlManager.Instance.MasterServerUrl.Compose(true));
            Connect();
        }

        protected override void Update()
        {
            base.Update();
            currentTick += Time.deltaTime;
            if (!(currentTick > syncInterval)) return;
            currentTick = 0;
            UpdateServerList();
        }

        public void UpdateServerList()
        {
            if (Status != ConnectionStatus.Connected) return;
            var entry = new ServerEntry()
            {
                label = Server.Instance.Name,
                port = Server.Instance.Port,
                playerCapacity = Server.Instance.PlayerCapacity,
                currentPlayers = GameAuthenticator.characterNameByClientId.Count
            };
            Send(new WebSocketMessage()
            {
                eventName = "update",
                data = JsonConvert.SerializeObject(entry)
            });
        }

        protected override void OnMessage(WebSocketMessage message)
        {
            Debug.Log(JsonConvert.SerializeObject(message));
            switch (message.eventName)
            {
                case "authorized":
                    UpdateServerList();
                    break;
                case "server-list":
                    ReceivedServerList(message);
                    break;
            }
        }

        private static void ReceivedServerList(WebSocketMessage message)
        {
            var list = JsonConvert.DeserializeObject<List<ServerEntry>>(message.data);
            MasterServerApi.Instance.ReceivedServerList(list);
        }
    }
}