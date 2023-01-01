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
        private WebSocketConnection connection => this;
        private float syncInterval = 10f;
        private float currentTick = 0;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            MessageReceived += OnMessage;
        }

        private void OnDisable()
        {
            MessageReceived -= OnMessage;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            MessageReceived -= OnMessage;
        }

        private void Start()
        {
            UpdateServerUrl(UrlManager.Instance.MasterServerUrl.Compose(true));
            Connect();
        }

        void OnMessage(WebSocketMessage message)
        {
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

        protected override void Update()
        {
            base.Update();
            currentTick += Time.deltaTime;
            if (!(currentTick > syncInterval)) return;
            currentTick = 0;
            if (connection == null) return;
            UpdateServerList();
        }

        public void UpdateServerList()
        {
            var entry = new ServerEntry()
            {
                label = Server.Instance.Name,
                port = Server.Instance.Port,
                playerCapacity = Server.Instance.PlayerCapacity,
                currentPlayers = GameAuthenticator.characterNameByClientId.Count
            };
            connection.Send(new WebSocketMessage()
            {
                eventName = "update",
                data = JsonConvert.SerializeObject(entry)
            });
        }

        private static void ReceivedServerList(WebSocketMessage message)
        {
            var list = JsonConvert.DeserializeObject<List<ServerEntry>>(message.data);
            MasterServerApi.Instance.ReceivedServerList(list);
        }
    }
}