using System.Threading.Tasks;
using Unisave.Facades;
using UnityEngine;

namespace ClaraMundi
{
    public class SessionManager : MonoBehaviour
    {
        public static SessionManager Instance;

        public Account PlayerAccount;
        public CharacterEntity PlayerCharacter;

        public string token;

        private void Awake()
        {
            if (Instance != null)
                DestroyImmediate(Instance.gameObject);
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Clear()
        {
            PlayerAccount = null;
            PlayerCharacter = null;
        }
    }
}