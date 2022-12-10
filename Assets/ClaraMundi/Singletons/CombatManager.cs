using UnityEngine;

namespace ClaraMundi
{
    public class CombatManager : MonoBehaviour
    {
        public static CombatManager Instance;

        private void Awake()
        {
            Instance = this;
        }
    }
}