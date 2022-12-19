using System;
using Unisave.Entities;

/*
 * This entity represents a player of your game. To learn how to add
 * player registration and authentication, check out the documentation:
 * https://unisave.cloud/docs/authentication
 *
 * If you don't need to register players, remove this class.
 */

public class AccountEntity : Entity
{
    public string email;
    public string password;
    public DateTime lastLoginAt = DateTime.UtcNow;
    public string token;
}
