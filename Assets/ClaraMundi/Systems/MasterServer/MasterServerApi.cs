using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ClaraMundi
{
    public class MasterServerApi : MonoBehaviour
    {
        public static MasterServerApi Instance;
        [ShowInInspector]
        public readonly Dictionary<string, List<ServerEntry>> serversByRegion = new();
        [ShowInInspector]
        public readonly Dictionary<string, ServerEntry> serversByLabel = new();

        public event Action<List<ServerEntry>> ServerListUpdate;

        private void Awake()
        {
            Instance = this;
        }

        public async Task GetServerList()
        {
            try
            {
                var list = await HttpRequest.Get<List<ServerEntry>>(UrlManager.Instance.MasterServerUrl.Compose(), "/master-server/servers");
                ReceivedServerList(list);
            }
            catch (Exception e)
            {
              Debug.LogWarning("Could not update server list");  
            }
        }
        public void ReceivedServerList(List<ServerEntry> servers)
        {
            serversByRegion.Clear();
            serversByLabel.Clear();
            foreach (var server in servers)
            {
                if (!serversByRegion.ContainsKey(server.region))
                    serversByRegion[server.region] = new();
                serversByRegion[server.region].Add(server);
                serversByLabel[server.label] = server;
            }

            ServerListUpdate?.Invoke(servers);
        }
    }
}