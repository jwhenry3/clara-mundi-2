using UnityEngine;

namespace ClaraMundi
{
    public class RepoManager : MonoBehaviour
    {
        public ItemRepo ItemRepo;
        public static RepoManager Instance;

        private void Awake()
        {
            Instance = this;
        }
    }
}