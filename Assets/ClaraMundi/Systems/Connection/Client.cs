using FishNet;
using FishNet.Managing;
using FishNet.Transporting;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClaraMundi
{
  public class Client : MonoBehaviour
  {
    [HideInInspector] public NetworkManager networkManager;
    public static Client Instance;

    public string UIScene = "WorldUI";

    public ServerEntry SelectedServer;

    private LocalConnectionState _clientState = LocalConnectionState.Stopped;

    private void Awake()
    {
      Instance = this;
      // authenticationScreen.SetActive(false);
      // characterScreen.SetActive(false);
      networkManager = InstanceFinder.NetworkManager;
      CheckAuthentication();
    }

    private void CheckAuthentication()
    {
    }

    private void Start()
    {
      Physics.IgnoreLayerCollision(3, 3);
      networkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
      SceneManager.LoadScene(UIScene, LoadSceneMode.Additive);
    }

    public void Connect()
    {
      // Debug.Log("Connect");
      if (_clientState != LocalConnectionState.Stopped) return;
      if (SessionManager.Instance.PlayerCharacter == null) return;
      var server = SelectedServer;
      // Debug.Log("Server: " + server?.host);
      if (server == null) return;
      networkManager.ClientManager.StartConnection(server.host, server.port);
    }

    public void Disconnect()
    {
      if (_clientState == LocalConnectionState.Started)
        networkManager.ClientManager.StopConnection();
    }

    void ClientManager_OnClientConnectionState(ClientConnectionStateArgs obj)
    {
      _clientState = obj.ConnectionState;
      // joinLabel.text = GetNextStateText(_clientState);
      if (obj.ConnectionState == LocalConnectionState.Stopped)
        ChatWindowUI.Instance.ClearMessages();
    }


    private void OnDestroy()
    {
      networkManager.ClientManager.OnClientConnectionState -= ClientManager_OnClientConnectionState;
    }

    private static string GetNextStateText(LocalConnectionState state)
    {
      return state switch
      {
        LocalConnectionState.Stopped => "Join",
        LocalConnectionState.Starting => "Starting Client",
        LocalConnectionState.Stopping => "Stopping Client",
        LocalConnectionState.Started => "Leave Server",
        _ => "Invalid"
      };
    }
  }
}