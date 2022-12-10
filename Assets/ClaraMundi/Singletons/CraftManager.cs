using UnityEngine;

namespace ClaraMundi
{
    public class CraftManager : MonoBehaviour
    {
        public static CraftManager Instance;

        private void Awake()
        {
            Instance = this;
        }
    }
}