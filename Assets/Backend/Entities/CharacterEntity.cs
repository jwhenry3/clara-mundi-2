using System;
using Unisave.Entities;

public class CharacterEntity : Entity
{
    public EntityReference<AccountEntity> Account;
    public string Name;
    public string Gender;
    public string Race;
    public DateTime LastConnected;
    public DateTime LastDisconnected;
}
