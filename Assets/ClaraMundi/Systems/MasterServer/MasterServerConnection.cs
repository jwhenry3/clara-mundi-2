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
        }

        private void Start()
        {
            connection.MessageReceived += OnMessage;
            connection.Connect();
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

        private void Update()
        {
            currentTick += Time.deltaTime;
            if (currentTick > syncInterval)
            {
                currentTick = 0;
            }
        }

        public void UpdateServerList()
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

        void ReceivedServerList(WebSocketMessage message)
        {
            var list = JsonConvert.DeserializeObject<List<ServerEntry>>(message.data);
            MasterServerApi.Instance.ReceivedServerList(list);
        }
    }
}