using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace ClaraMundi
{
  public class ServerListUI : MonoBehaviour
  {
    public static ServerListUI Instance;
    public Transform ServerListContainer;
    public TextMeshProUGUI StatusText;

    public ServerUI ServerPrefab;

    public ServerEntry SelectedServer;

    private void OnEnable()
    {
      Instance = this;
      LoadServers();
    }

    public async void LoadServers()
    {
      StatusText.text = "Loading servers...";
      foreach (Transform child in ServerListContainer)
        Destroy(child.gameObject);
      await MasterServerApi.Instance.GetServerList();
      var servers = MasterServerApi.Instance.serversByLabel.ToList();
      foreach (var kvp in servers)
      {
        var instance = Instantiate(ServerPrefab, ServerListContainer, false);
        instance.StatusText.text = kvp.Value.status ? "Up" : "Down";
        instance.NameText.text = kvp.Value.label;
        instance.CapacityText.text = $"{kvp.Value.currentPlayers}/{kvp.Value.playerCapacity}";
        instance.Server = kvp.Value;
      }

      StatusText.text = servers.Count > 0 ? "" : "No servers currently online";
    }

    public void EnterGame()
    {
      LobbyUI.Instance.OnEnterGame();
    }
  }
}