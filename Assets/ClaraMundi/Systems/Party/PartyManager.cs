using UnityEngine;

namespace ClaraMundi
{
    public class PartyManager : MonoBehaviour
    {
        public static PartyManager Instance;
        private void Awake()
        {
            Instance = this;
        }



        public void ServerSendMessage(ChatMessage message)
        {
            var player = PlayerManager.Instance.LocalPlayer;
            if (player == null) return;
            player.Party.SendMessage(message);
            // send chat message to the party facet
        }
        

    }
}