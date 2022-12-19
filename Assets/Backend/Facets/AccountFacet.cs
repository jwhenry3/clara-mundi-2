using System;
using Unisave.Facades;
using Unisave.Facets;
using Unisave.Utils;

/*
 * EmailAuthentication template - v0.9.1
 * -------------------------------------
 *
 * This facet handles player login via email and password and player logout.
 *
 * You can extend the PlayerHasLoggedIn(...) method to perform logic
 * after a successful login attempt.
 */

public class AccountFacet : Facet
{
    /// <summary>
    /// Call this from your login form
    /// </summary>
    /// <param name="email">Player's email</param>
    /// <param name="password">Player's password</param>
    /// <returns>True when the login succeeds</returns>
    public AccountEntity GetAccount()
    {
        // obtain authenticated player ID from the session
        // and load player data from the database
        AccountEntity account = Auth.GetPlayer<AccountEntity>();

        // send the data back to the game client
        return account;
    }

    public void LogAccountOut(string serverToken, string accountId)
    {
        var account = DB.Find<AccountEntity>(accountId);
        account.token = "";
        account.Save();
        foreach (var character in DB.TakeAll<CharacterEntity>()
                     .Filter((entity) => entity.Account.TargetId == accountId && entity.LastConnected > entity.LastDisconnected).Get())
        {
            character.LastDisconnected = DateTime.UtcNow;
            character.Save();
        }
    }
}

