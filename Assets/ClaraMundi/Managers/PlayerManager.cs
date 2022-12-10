using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClaraMundi
{
    public class PlayerManager : MonoBehaviour
    {
        public Player LocalPlayer;
        public event Action<Player> OnPlayerChange;
        public readonly Dictionary<string, Player> Players = new();
        public readonly Dictionary<string, Player> PlayersByName = new();
        public static PlayerManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        public void ChangeLocalPlayer(Player player)
        {
            LocalPlayer = player;
            OnPlayerChange?.Invoke(player);
        }

        public Player GetByName(string playerName)
        {
            return PlayersByName.ContainsKey(playerName.ToLower()) ? PlayersByName[playerName.ToLower()] : null;
        }
        public Player GetById(string id)
        {
            return Players.ContainsKey(id) ? Players[id] : null;
        }
    }
}