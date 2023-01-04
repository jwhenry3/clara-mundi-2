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
        public Character Character => Entity.Character;

        public GCharacter Body;
        [HideInInspector] public Entity Entity { get; private set; }
        [HideInInspector] public string entityId => Entity.entityId;
        [HideInInspector] public StatsController Stats { get; protected set; }
        [HideInInspector] public InventoryController Inventory { get; protected set; }
        [HideInInspector] public EquipmentController Equipment { get; protected set; }
        [HideInInspector] public AlertController Alerts { get; protected set; }
        [HideInInspector] public ChatController Chat { get; protected set; }
        [HideInInspector] public PartyController Party { get; protected set; }
        [HideInInspector] public QuestController Quests { get; protected set; }


        private NetworkManager networkManager;

        private void Awake()
        {
            networkManager = InstanceFinder.NetworkManager;
            Body = GetComponentInChildren<GCharacter>();
            Entity = GetComponent<Entity>();
            Stats = GetComponentInChildren<StatsController>();
            Inventory = GetComponentInChildren<InventoryController>();
            Equipment = GetComponentInChildren<EquipmentController>();
            Alerts = GetComponentInChildren<AlertController>();
            Chat = GetComponentInChildren<ChatController>();
            Party = GetComponentInChildren<PartyController>();
            Quests = GetComponentInChildren<QuestController>();
            Entity.OnStarted += OnNetStarted;
        }

        private void OnNetStarted()
        {
            ClientId = Entity.Owner.ClientId;
            PlayerManager.Instance.Players[Entity.entityId] = this;
            PlayerManager.Instance.PlayersByName[Character.name.ToLower()] = this;
            if (!Entity.IsOwner) return;
            PlayerManager.Instance.ChangeLocalPlayer(this);
            NetStarted?.Invoke();
            Body.IsPlayer = true;
            CameraManager.Instance.UsePlayerCamera();
        }

        private void OnDestroy()
        {
            if (networkManager.IsServer)
            {
                // clean up authenticator references to player names connected
                GameAuthenticator.RemovePlayerReference(ClientId, Entity.entityName);
            }

            if (Body.IsPlayer)
                CameraManager.Instance.UseLoginCamera();
            Entity.OnStarted -= OnNetStarted;
            if (PlayerManager.Instance.Players.ContainsKey(Entity.entityId))
                PlayerManager.Instance.Players.Remove(Entity.entityId);
            if (PlayerManager.Instance.PlayersByName.ContainsKey(Entity.entityName.ToLower()))
                PlayerManager.Instance.PlayersByName.Remove(Entity.entityName.ToLower());
            if (PlayerManager.Instance.LocalPlayer == this)
                PlayerManager.Instance.ChangeLocalPlayer(null);
        }

        public static string GetClickableName(string characterName)
        {
            return $"<link=\"player:{characterName}\">{characterName}</link>";
        }
    }
}