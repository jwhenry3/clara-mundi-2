using TMPro;
using UnityEngine;

namespace ClaraMundi
{
    public class ServerUI : MonoBehaviour
    {
        public ServerEntry Server;
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI StatusText;
        public TextMeshProUGUI CapacityText;

        public void OnClick()
        {
            Client.Instance.SelectedServer = Server;
            ServerListUI.Instance.EnterGame();
            
        }
    }
}