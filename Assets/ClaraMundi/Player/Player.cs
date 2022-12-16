﻿using UnityEngine;
using FishNet.Managing;
using FishNet;

namespace ClaraMundi
{
    public class Player : MonoBehaviour
    {
        public int ClientId;
        [HideInInspector]
        public Entity Entity { get; private set; }
        [HideInInspector]
        public string entityId => Entity.entityId;
        [HideInInspector]
        public StatsController Stats { get; protected set; }
        [HideInInspector]
        public InventoryController Inventory { get; protected set; }
        [HideInInspector]
        public EquipmentController Equipment { get; protected set; }
        [HideInInspector]
        public AlertController Alerts { get; protected set; }
        [HideInInspector]
        public ChatController Chat { get; protected set;  }
        [HideInInspector]
        public PartyController Party { get; protected set;  }
        [HideInInspector]
        public QuestController Quests { get; protected set;  }


        private NetworkManager networkManager;

        private void Awake()
        {
            networkManager = InstanceFinder.NetworkManager;
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
            PlayerManager.Instance.PlayersByName[Entity.entityName.ToLower()] = this;
            if (!Entity.IsOwner) return;
            PlayerManager.Instance.ChangeLocalPlayer(this);
        }

        private void OnDestroy()
        {
            if (networkManager.IsServer)
            {
                // clean up authenticator references to player names connected
                GameAuthenticator.RemovePlayerReference(ClientId, Entity.entityName);
            }
            Entity.OnStarted -= OnNetStarted;
            if (PlayerManager.Instance.Players.ContainsKey(Entity.entityId))
                PlayerManager.Instance.Players.Remove(Entity.entityId);
            if (PlayerManager.Instance.PlayersByName.ContainsKey(Entity.entityName.ToLower()))
                PlayerManager.Instance.PlayersByName.Remove(Entity.entityName.ToLower());
            if (PlayerManager.Instance.LocalPlayer == this)
                PlayerManager.Instance.ChangeLocalPlayer(null);
        }

        public static string GetClickableName(string entityId)
        {
            if (!PlayerManager.Instance.Players.ContainsKey(entityId)) return "";
            var player = PlayerManager.Instance.Players[entityId];
            return $"<link=\"player:{entityId}\">{player.Entity.entityName}</link>";
        }
        public static string GetClickableNameByName(string name)
        {
            if (!PlayerManager.Instance.PlayersByName.ContainsKey(name.ToLower())) return "";
            var player = PlayerManager.Instance.PlayersByName[name.ToLower()];
            return $"<link=\"player:{player.Entity.entityId}\">{player.Entity.entityName}</link>";
        }
    }
}