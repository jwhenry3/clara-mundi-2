using System;
using UnityEngine;
using FishNet.Managing;
using FishNet;
using FishNet.Object;
using GCharacter = GameCreator.Runtime.Characters.Character;

namespace ClaraMundi
{
  public class Player : NetworkBehaviour
  {
    public event Action NetStarted;
    public int ClientId;
    public Character Character => Entity.Character.Value;

    public GCharacter Body;
    public Entity Entity { get; private set; }
    public string entityId => Entity.entityId.Value;
    public StatsController Stats { get; protected set; }
    public InventoryController Inventory { get; protected set; }
    public EquipmentController Equipment { get; protected set; }
    public AlertController Alerts { get; protected set; }
    public ChatController Chat { get; protected set; }
    public PartyController Party { get; protected set; }
    public QuestController Quests { get; protected set; }
    public ClickToMoveController Movement { get; protected set; }


    private NetworkManager networkManager;

    private void Awake()
    {
      networkManager = InstanceFinder.NetworkManager;
      Body = GetComponentInChildren<GCharacter>();
      Entity = GetComponent<Entity>();
      Stats = GetComponentInChildren<StatsController>();
      Stats.player = this;
      Inventory = GetComponentInChildren<InventoryController>();
      Inventory.player = this;
      Equipment = GetComponentInChildren<EquipmentController>();
      Equipment.player = this;
      Alerts = GetComponentInChildren<AlertController>();
      Chat = GetComponentInChildren<ChatController>();
      Chat.player = this;
      Party = GetComponentInChildren<PartyController>();
      Party.player = this;
      Quests = GetComponentInChildren<QuestController>();
      Quests.player = this;
      Movement = GetComponentInChildren<ClickToMoveController>();
      Movement.player = this;
      Entity.OnStarted += OnNetStarted;
    }

    private void OnNetStarted()
    {
      ClientId = Entity.Owner.ClientId;
      PlayerManager.Instance.Players[Entity.entityId.Value] = this;
      PlayerManager.Instance.PlayersByName[Character.name.ToLower()] = this;
      if (!Entity.IsOwner) return;
      PlayerManager.Instance.ChangeLocalPlayer(this);
      NetStarted?.Invoke();
      Body.IsPlayer = true;
      CameraManager.Instance.UsePlayerCamera(this);
      InputManager.Instance.World.Enable();
    }

    private void OnDestroy()
    {
      if (networkManager.IsServerStarted)
      {
        // clean up authenticator references to player names connected
        GameAuthenticator.RemovePlayerReference(ClientId, Entity.entityName.Value);
      }

      if (Body.IsPlayer)
      {
        CameraManager.Instance.UseLoginCamera();
        InputManager.Instance.World.Disable();
      }

      Entity.OnStarted -= OnNetStarted;
      if (PlayerManager.Instance.Players.ContainsKey(Entity.entityId.Value))
        PlayerManager.Instance.Players.Remove(Entity.entityId.Value);
      if (PlayerManager.Instance.PlayersByName.ContainsKey(Entity.entityName.Value.ToLower()))
        PlayerManager.Instance.PlayersByName.Remove(Entity.entityName.Value.ToLower());
      if (PlayerManager.Instance.LocalPlayer == this)
        PlayerManager.Instance.ChangeLocalPlayer(null);
    }

    public static string GetClickableName(string characterName)
    {
      return $"<link=\"player:{characterName}\">{characterName}</link>";
    }

    public void ServerChangeClass(string classId)
    {
      if (!Entity.IsServerStarted) return;
      if (!Entity.Classes.ContainsKey(classId)) return;
      if (Entity.CurrentClass != null)
      {
        Entity.CurrentClass.isCurrent = false;
        Equipment.ServerUnequipAll();
        UpdateClass(Entity.CurrentClass.classId);
      }

      Entity.CurrentClassId.Value = classId;
      if (Entity.CurrentClass == null)
      {
        Debug.LogWarning("Cannot load current class details");
        return;
      }
      Entity.CurrentClass.isCurrent = true;
      Stats.Level.Value = Entity.CurrentClass.level;
      Stats.Experience.Value = Entity.CurrentClass.exp;
      UpdateClass(Entity.CurrentClass.classId);
      Equipment.ServerEquipAll(Entity.CurrentClass.equipment);
    }

    private void UpdateClass(string classId)
    {
      var previous = Entity.Classes[classId];
      Entity.Classes[classId] = new CharacterClass()
      {
        classId = classId,
        level = previous.level,
        exp = previous.exp,
        isCurrent = false,
        equipment = previous.equipment
      };
    }
  }
}