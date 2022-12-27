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
    }
    public class Authorization
    {
        public static async void Login(string username, string password)
        {
            var content = new Dictionary<string, string>() { { "email", username }, { "password", password } };
            var response = await HttpRequest.Post<AuthResponse>(UrlManager.Instance.LoginServerUrl.Compose(),
                "/login-server/login",
                JsonConvert.SerializeObject(content)
            );
            Debug.Log(response.status);
            
        }

        public static void Register(string username, string password)
        {
        }
    }
}