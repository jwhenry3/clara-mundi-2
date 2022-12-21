using System;
using Unisave.Entities;
using UnityEngine;

public class CharacterEntity : Entity
{
    public EntityReference<AccountEntity> Account;
    public string Name;
    public string Gender;
    public string Race;
    public string Area;
    public Vector3 Position;

    public int Level;
    public int TotalExp;

    public DateTime LastConnected;
    public DateTime LastDisconnected;
}
