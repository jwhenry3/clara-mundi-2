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
            var party = PlayerManager.Instance.LocalPlayer.Party.Party;
            if (party == null) return;
            // send chat message to the party facet
        }
        

    }
}