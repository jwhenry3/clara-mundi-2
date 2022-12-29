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
    public class AuthResponse : OperationResponse
    {
        public Account account;
    }

    [Serializable]
    public class CharactersResponse : OperationResponse
    {
        public List<Character> characters;
    }

    [Serializable]
    public class CharacterResponse : OperationResponse
    {
        public Character character;
    }


    public class LobbyApi
    {
        public static async Task<AuthResponse> Login(string username, string password)
        {
            var content = new Dictionary<string, string>() { { "email", username }, { "password", password } };
            return await HttpRequest.Post<AuthResponse>(UrlManager.Instance.LoginServerUrl.Compose(),
                "/login-server/login",
                JsonConvert.SerializeObject(content)
            );
        }

        public static async Task<AuthResponse> Register(string username, string password)
        {
            var content = new Dictionary<string, string>() { { "email", username }, { "password", password } };
            return await HttpRequest.Post<AuthResponse>(UrlManager.Instance.LoginServerUrl.Compose(),
                "/login-server/register",
                JsonConvert.SerializeObject(content)
            );
        }

        public static async Task<CharacterResponse> VerifyCharacter(string token, string characterName,
            bool isConnecting = false)
        {
            return await HttpRequest.Get<CharacterResponse>(UrlManager.Instance.LoginServerUrl.Compose(),
                $"/login-server/characters/{characterName}/verify?token=" + token + "&isConnecting=" +
                (isConnecting ? 1 : 0)
            );
        }

        public static async Task<CharacterResponse> LogoutCharacter(string serverToken, string characterName)
        {
            return await HttpRequest.Get<CharacterResponse>(UrlManager.Instance.LoginServerUrl.Compose(),
                $"/login-server/characters/{characterName}/logout?token=" + serverToken
            );
        }

        public static async Task<CharactersResponse> GetCharacters()
        {
            var token = SessionManager.Instance.PlayerAccount.token;
            return await HttpRequest.Get<CharactersResponse>(UrlManager.Instance.LoginServerUrl.Compose(),
                "/login-server/characters/list?token=" + token
            );
        }

        public static async Task<CharacterResponse> CreateCharacter(string name, string gender,
            string race)
        {
            var token = SessionManager.Instance.PlayerAccount.token;
            var content = new Dictionary<string, string>()
                { { "token", token }, { "name", name }, { "race", race }, { "gender", gender } };
            return await HttpRequest.Post<CharacterResponse>(UrlManager.Instance.LoginServerUrl.Compose(),
                "/login-server/characters/create",
                JsonConvert.SerializeObject(content)
            );
        }

        public static async Task<OperationResponse> DeleteCharacter(string name)
        {
            var token = SessionManager.Instance.PlayerAccount.token;
            return await HttpRequest.Delete<OperationResponse>(UrlManager.Instance.LoginServerUrl.Compose(),
                $"/login-server/characters/{name}?token=" + token + "&name=" + name
            );
        }
    }
}