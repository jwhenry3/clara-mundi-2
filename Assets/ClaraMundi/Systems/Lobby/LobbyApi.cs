using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace ClaraMundi
{
  [Serializable]
  public class OperationResponse
  {
    public bool status;
    public string reason;
  }

  [Serializable]
  public class CharacterData {
    public string name;
    public string gender;
    public string race;
    public string area;
    public int? level;
    public int? exp;
    public string classid;
  }

  [Serializable]
  public class AuthResponse : OperationResponse
  {
    public Account account;
  }

  [Serializable]
  public class CharactersResponse : OperationResponse
  {
    public List<CharacterData> characters;
  }

  [Serializable]
  public class CharacterResponse : OperationResponse
  {
    public CharacterData character;
  }

  public class LoginRequest {
    public string email;
    public string password;
  }
  public class CreateCharacterRequest {
    public string token;
    public string name;
    public string race;
    public string gender;
    public string startingClass;
  }

  public class LobbyApi
  {

    public static string BaseUrl
    {
      get => UrlManager.Instance.LoginServerUrl.Compose();
    }

    public static async Task<AuthResponse> Login(string username, string password)
    {
      var content = new LoginRequest() { email = username , password = password };
      return await HttpRequest.Post<AuthResponse>(BaseUrl,
          "/login-server/login",
          JsonConvert.SerializeObject(content)
      );
    }

    public static async Task<AuthResponse> Register(string username, string password)
    {
      var content = new LoginRequest() { email = username , password = password };
      return await HttpRequest.Post<AuthResponse>(BaseUrl,
          "/login-server/register",
          JsonConvert.SerializeObject(content)
      );
    }

    public static async Task<CharacterResponse> VerifyCharacter(string token, string characterName,
        bool isConnecting = false)
    {
      Debug.Log($"/login-server/characters/{characterName}/verify?token=" + token + "&isConnecting=" +
          (isConnecting ? 1 : 0));
      return await HttpRequest.Get<CharacterResponse>(BaseUrl,
          $"/login-server/characters/{characterName}/verify?token=" + token + "&isConnecting=" +
          (isConnecting ? 1 : 0)
      );
    }

    public static async Task<CharacterResponse> LogoutCharacter(string serverToken, string characterName)
    {
      return await HttpRequest.Get<CharacterResponse>(BaseUrl,
          $"/login-server/characters/{characterName}/logout?token=" + serverToken
      );
    }

    public static async Task<CharactersResponse> GetCharacters()
    {
      var token = SessionManager.Instance.PlayerAccount.token;
      return await HttpRequest.Get<CharactersResponse>(BaseUrl,
          "/login-server/characters/list?token=" + token
      );
    }

    public static async Task<CharacterResponse> CreateCharacter(
        string name,
        string gender,
        string race,
        string className
        )
    {
      var content = new CreateCharacterRequest() {
        token = SessionManager.Instance.PlayerAccount.token,
        name = name,
        race = race,
        gender = gender,
        startingClass = className
      };
      return await HttpRequest.Post<CharacterResponse>(BaseUrl,
          "/login-server/characters/create",
          JsonConvert.SerializeObject(content)
      );
    }

    public static async Task<OperationResponse> DeleteCharacter(string name)
    {
      var token = SessionManager.Instance.PlayerAccount.token;
      return await HttpRequest.Delete<OperationResponse>(BaseUrl ,
          $"/login-server/characters/{name}?token=" + token + "&name=" + name
      );
    }
  }
}