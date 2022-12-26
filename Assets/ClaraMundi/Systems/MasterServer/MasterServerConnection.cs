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
                case "authorize":
                {
                    SendAuthorization();
                    break;
                }
                case "authorized":
                    Debug.Log("Authorized");
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

        void SendAuthorization()
        {
            var data = new Dictionary<string, string> { { "token", "" } };
            connection.Send(new WebSocketMessage()
            {
                eventName = "authorize",
                data = data
            });
        }

        void ReceivedServerList(WebSocketMessage message)
        {
            if (!message.data.ContainsKey("list")) return;
            var list = JsonConvert.DeserializeObject<List<ServerEntry>>(message.data["list"]);
            MasterServerApi.Instance.ReceivedServerList(list);
        }
    }
}