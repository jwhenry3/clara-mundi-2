using UnityEngine;

namespace ClaraMundi
{
    public class AlertManager : MonoBehaviour
    {
        public static AlertManager Instance;

        public Alert AlertPrefab;

        private void Awake()
        {
            if (Instance != null)
                Destroy(Instance);
            Instance = this;
        }
        public void AddMessage(AlertMessage message)
        {
            Alert instance = Instantiate(AlertPrefab, transform, true);
            instance.SetMessage(message);
        }
    }
}