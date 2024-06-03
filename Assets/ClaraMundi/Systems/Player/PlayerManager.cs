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

    public Player GetPlayer(string idOrName)
    {
      if (Players.ContainsKey(idOrName))
        return Players[idOrName];
      if (PlayersByName.ContainsKey(idOrName.ToLower().Replace(" ", "")))
        return PlayersByName[idOrName.ToLower().Replace(" ", "")];
      return null;
    }

    public void SetPlayer(Player player)
    {
      Players[player.Entity.entityId.Value] = player;
      Debug.Log(player.Character.name);
      PlayersByName[player.Character.name.ToLower().Replace(" ", "")] = player;
    }
  }
}