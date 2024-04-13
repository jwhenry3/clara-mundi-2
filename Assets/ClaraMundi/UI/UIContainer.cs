using FishNet.Managing;
using UnityEngine;

namespace ClaraMundi
{
  [CreateAssetMenu(fileName = "UIContainer", menuName = "Clara Mundi/React/UIContainer")]
  public class UIContainer : ScriptableObject
  {
    public EntityManager Entity;
    public CombatManager Combat;
    public CraftManager Craft;
    public ItemManager Item;
    public QuestManager Quest;
    public PartyManager Party;
    public PlayerManager Player;
    public ChatManager Chat;

    public MasterServerApi Api;

    public NetworkManager Network;
  }
}