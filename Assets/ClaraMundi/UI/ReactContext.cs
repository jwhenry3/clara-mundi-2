using FishNet.Managing;
using UnityEngine;

namespace ClaraMundi
{
  public class ReactContext : MonoBehaviour
  {
    public UIContainer UI;
    public NetworkManager NetworkManager;
    void Start()
    {
      UI.Api = GetComponent<MasterServerApi>();
      UI.Chat = GetComponent<ChatManager>();
      UI.Combat = GetComponent<CombatManager>();
      UI.Craft = GetComponent<CraftManager>();
      UI.Entity = GetComponent<EntityManager>();
      UI.Item = GetComponent<ItemManager>();
      UI.Network = NetworkManager;
      UI.Player = GetComponent<PlayerManager>();
      UI.Party = GetComponent<PartyManager>();
      UI.Quest = GetComponent<QuestManager>();
    }
  }
}