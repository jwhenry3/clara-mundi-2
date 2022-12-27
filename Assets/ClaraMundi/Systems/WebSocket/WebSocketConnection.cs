using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NativeWebSocket;
using Newtonsoft.Json;


namespace ClaraMundi
{
    public class WebSocketMessage
    {
        public string eventName;
        public string data;
    }

    public enum ConnectionStatus
    {
        Connecting,
        Connected,
        Disconnected,
    }

    public class WebSocketConnection : MonoBehaviour
    {
        public string Label;
        public string serverUrl;
        public string authToken;
        public ConnectionStatus Status { get; protected set; }
        private WebSocket websocket;

        private readonly WebSocketCloseCode[] reconnectOn = new[]
        {
            WebSocketCloseCode.Abnormal
        };

        public event Action<WebSocketMessage> MessageReceived;

        private void Awake()
        {
            CreateSocket();
        }

        private async void CreateSocket()
        {
            if (websocket != null)
            {
                websocket.OnOpen -= OnConnected;
                websocket.OnError -= OnError;
                websocket.OnClose -= OnDisconnected;
                websocket.OnMessage -= OnMessage;
                if (Status == ConnectionStatus.Connected)
                    await websocket.Close();
            }

            Status = ConnectionStatus.Disconnected;
            if (string.IsNullOrEmpty(authToken))
                websocket = new WebSocket(serverUrl);
            else
                websocket = new WebSocket(serverUrl + "?token=" + authToken);

            websocket.OnOpen += OnConnected;
            websocket.OnError += OnError;
            websocket.OnClose += OnDisconnected;
            websocket.OnMessage += OnMessage;
        }

        private void OnDestroy()
        {
            websocket.OnOpen -= OnConnected;
            websocket.OnError -= OnError;
            websocket.OnClose -= OnDisconnected;
            websocket.OnMessage -= OnMessage;
        }

        public async void Connect()
        {
#if UNITY_EDITOR
            Debug.Log($"{Label} connecting...");
#endif
            Status = ConnectionStatus.Connecting;
            await websocket.Connect();
        }

        void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            websocket?.DispatchMessageQueue();
#endif
        }

        public void Send(WebSocketMessage message)
        {
            websocket.SendText(JsonConvert.SerializeObject(message).Replace("\"eventName\"", "\"event\""));
        }

        private async void OnApplicationQuit()
        {
            await websocket.Close();
        }

        private void OnMessage(byte[] data)
        {
            var dataString = Encoding.UTF8.GetString(data);
#if UNITY_EDITOR
            Debug.Log($"Message Received from {Label}: " + dataString);
#endif
            try
            {
                MessageReceived?.Invoke(
                    JsonConvert.DeserializeObject<WebSocketMessage>(dataString.Replace("\"event\"", "\"eventName\"")));
            }
            catch (Exception e)
            {
                Debug.LogWarning("Unable to deserialize the message: " + dataString);
            }
        }

        private async void OnDisconnected(WebSocketCloseCode closecode)
        {
            Status = ConnectionStatus.Disconnected;
            if (reconnectOn.Contains(closecode) && isActiveAndEnabled)
            {
#if UNITY_EDITOR
                Debug.Log($"{Label} Disconnected abnormally, reconnecting...");
#endif
                await Task.Delay(1000);
                await websocket.Connect();
                return;
            }
#if UNITY_EDITOR
            Debug.Log($"{Label} Disconnected");
#endif
        }

        private void OnError(string errormsg)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{Label} Error: " + errormsg);
#endif
        }

        void OnConnected()
        {
            Status = ConnectionStatus.Connected;
#if UNITY_EDITOR
            Debug.Log($"{Label} Connected!");
#endif
        }
    }
}