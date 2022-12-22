using System.Threading.Tasks;
using Unisave.Facades;
using UnityEngine;

namespace ClaraMundi
{
    public class SessionManager : MonoBehaviour
    {
        public static SessionManager Instance;

        public AccountEntity PlayerAccount;
        public CharacterEntity PlayerCharacter;

        private void Awake()
        {
            if (Instance != null)
                DestroyImmediate(Instance.gameObject);
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        public async Task<bool> GetAccount()
        {
            PlayerAccount = await OnFacet<AccountFacet>.CallAsync<AccountEntity>(
                nameof(AccountFacet.GetAccount)
            );
            return PlayerAccount != null;
        }
    }
}