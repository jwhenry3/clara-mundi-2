using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace ClaraMundi
{
    [Serializable]
    public class AuthResponse
    {
        public bool status;
        public string reason;
        public Account account;
    }

    public class Authentication
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
    }
}