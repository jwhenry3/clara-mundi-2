using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ClaraMundi
{
    public class ServerEntry
    {
        public string label;
        public string region;
        public string host;
        public ushort port;
        public bool status;
        public int playerCapacity;
        public int currentPlayers;
    }

    public class MasterServerConnection : MonoBehaviour
    {
        private WebSocketConnection connection;
        private float syncInterval = 10f;
        private float currentTick = 0;

        private void Awake()
        {
            connection = GetComponent<WebSocketConnection>();
            connection.MessageReceived += OnMessage;
        }

        private void OnDestroy()
        {
            connection.MessageReceived -= OnMessage;
        }

        private void Start()
        {
            connection.UpdateServerUrl(UrlManager.Instance.MasterServerUrl.Compose(true));
            connection.Connect();
        }

        void OnMessage(WebSocketMessage message)
        {
            switch (message.eventName)
            {
                case "authorized":
                    UpdateServerList();
                    Authorization.Login("test", "password");
                    break;
                case "server-list":
                    ReceivedServerList(message);
                    break;
            }
        }

        private void Update()
        {
            currentTick += Time.deltaTime;
            if (!(currentTick > syncInterval)) return;
            currentTick = 0;
            UpdateServerList();
        }

        private void UpdateServerList()
        {
            var entry = new ServerEntry()
            {
                label = Server.Instance.Name,
                region = Server.Instance.Region,
                port = Server.Instance.Port,
                playerCapacity = Server.Instance.PlayerCapacity,
                currentPlayers = PlayerManager.Instance.Players.Count
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