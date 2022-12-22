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
    public float Rotation;

    public int Level;
    public int TotalExp;

    public DateTime LastConnected;
    public DateTime LastDisconnected;
    
    public bool HasConnectedBefore;
}
[Serializable]
public class CharacterModel
{
    public string CharacterId;
    public string Name;
    public string Gender;
    public string Race;
    public string Area;
    public Vector3 Position;
    public float Rotation;

    public int Level;
    public int TotalExp;
}
