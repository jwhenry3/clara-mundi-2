using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ClaraMundi
{
  public class MasterServerApi : MonoBehaviour
  {
    public static string BaseUrl
    {
      get => UrlManager.Instance.LoginServerUrl.Compose();
    }
    public static MasterServerApi Instance;
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
        var list = await HttpRequest.Get<List<ServerEntry>>(BaseUrl, "/master-server/servers");
        ReceivedServerList(list);
      }
      catch (Exception)
      {
        Debug.LogWarning("Could not update server list");
      }
    }
    public void ReceivedServerList(List<ServerEntry> servers)
    {
      serversByLabel.Clear();
      foreach (var server in servers)
      {
        serversByLabel[server.label] = server;
      }

      ServerListUpdate?.Invoke(servers);
    }
  }
}