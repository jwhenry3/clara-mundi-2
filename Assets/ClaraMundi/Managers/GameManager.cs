using FishNet;
using FishNet.Managing;
using FishNet.Managing.Client;
using FishNet.Managing.Scened;
using FishNet.Managing.Server;
using UnityEngine;

namespace ClaraMundi
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        
        // Game-Specific Managers
        public PlayerManager Player => PlayerManager.Instance;
        public Player LocalPlayer => PlayerManager.Instance.LocalPlayer;
        public RepoManager Repo => RepoManager.Instance;
        public UIManager UI => UIManager.Instance;
        public EntityManager Entity => EntityManager.Instance;
        public ItemManager Item => ItemManager.Instance;
        public CombatManager Combat => CombatManager.Instance;
        public QuestManager Quest => QuestManager.Instance;
        public CraftManager Craft => CraftManager.Instance;
        public ChatManager Chat => ChatManager.Instance;
        
        // Networking Managers
        public NetworkManager Network => InstanceFinder.NetworkManager;
        public ServerManager Server => InstanceFinder.ServerManager;
        public ClientManager Client => InstanceFinder.ClientManager;
        public SceneManager Scene => InstanceFinder.SceneManager;
        
        private void Awake()
        {
            if (Instance != null)
                DestroyImmediate(Instance.gameObject);
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}