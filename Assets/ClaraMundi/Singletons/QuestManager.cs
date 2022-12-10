using UnityEngine;

namespace ClaraMundi
{
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance;

        private void Awake()
        {
            Instance = this;
        }
    }
}