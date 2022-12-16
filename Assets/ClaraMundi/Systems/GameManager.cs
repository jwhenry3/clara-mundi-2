using UnityEngine;

namespace ClaraMundi
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        private void Awake()
        {
            if (Instance != null)
                DestroyImmediate(Instance.gameObject);
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}