﻿using System;
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
        public bool debugLog;
        public ConnectionStatus Status { get; protected set; }
        protected WebSocket websocket;

        protected readonly WebSocketCloseCode[] reconnectOn = new[]
        {
            WebSocketCloseCode.Abnormal
        };


        public void UpdateServerUrl(string url)
        {
            var parts = url.Split("://");
            var protocol = parts.Length > 1 ? parts[0] : "http";
            var hostAndPort = parts.Last().Split(":");
            var port = hostAndPort.Length > 1 ? hostAndPort.Last() : "";
            var host = hostAndPort.First();
            serverUrl = "ws://" + host;
            if (protocol == "https")
                serverUrl = "wss://" + host;
            if (port != "")
                serverUrl += ":" + port;
            else
                serverUrl += ":" + (protocol == "https" ? "443" : "80");
        }

        protected virtual async void CreateSocket()
        {
            if (websocket != null)
            {
                websocket.OnOpen -= OnConnected;
                websocket.OnError -= OnError;
                websocket.OnClose -= OnDisconnected;
                websocket.OnMessage -= WS_OnMessage;
                try
                {
                    if (Status == ConnectionStatus.Connected)
                        await websocket.Close();
                }
                catch (Exception e)
                {
                    // do nothing
                }
            }

            Status = ConnectionStatus.Disconnected;
            if (string.IsNullOrEmpty(authToken))
                websocket = new WebSocket(serverUrl);
            else
                websocket = new WebSocket(serverUrl + "?token=" + authToken);

            websocket.OnOpen += OnConnected;
            websocket.OnError += OnError;
            websocket.OnClose += OnDisconnected;
            websocket.OnMessage += WS_OnMessage;
        }

        protected virtual void OnDestroy()
        {
            websocket.OnOpen -= OnConnected;
            websocket.OnError -= OnError;
            websocket.OnClose -= OnDisconnected;
            websocket.OnMessage -= WS_OnMessage;
        }

        public async void Connect()
        {
            if (websocket == null)
                CreateSocket();
            if (debugLog) Debug.Log($"{Label} connecting...");
            Status = ConnectionStatus.Connecting;
            await websocket?.Connect()!;
        }

        protected virtual void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            websocket?.DispatchMessageQueue();
#endif
        }

        public void Send(WebSocketMessage message)
        {
            websocket.SendText(JsonConvert.SerializeObject(message).Replace("\"eventName\"", "\"event\""));
        }

        protected async void OnApplicationQuit()
        {
            await websocket.Close();
        }

        private void WS_OnMessage(byte[] data)
        {
            var dataString = Encoding.UTF8.GetString(data);
            if (debugLog) Debug.Log($"Message Received from {Label}: " + dataString);
            try
            {
                OnMessage(
                    JsonConvert.DeserializeObject<WebSocketMessage>(dataString.Replace("\"event\"", "\"eventName\"")));
            }
            catch (Exception e)
            {
                Debug.LogWarning("Unable to deserialize the message: " + dataString);
            }
        }

        protected virtual void OnMessage(WebSocketMessage message)
        {
        }

        protected virtual async void OnDisconnected(WebSocketCloseCode closecode)
        {
            Status = ConnectionStatus.Disconnected;
            if (reconnectOn.Contains(closecode) && isActiveAndEnabled)
            {
                if (debugLog) Debug.Log($"{Label} Disconnected abnormally, reconnecting...");
                await Task.Delay(1000);
                Connect();
                return;
            }

            if (debugLog) Debug.Log($"{Label} Disconnected");
        }

        private void OnError(string errormsg)
        {
            if (debugLog) Debug.LogWarning($"{Label} Error: " + errormsg);
        }

        void OnConnected()
        {
            Status = ConnectionStatus.Connected;
            if (debugLog) Debug.Log($"{Label} Connected!");
        }
    }
}