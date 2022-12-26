using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace ClaraMundi
{
    public class PartyInviteRequestUI : MonoBehaviour
    {
        public string characterName;
        public TextMeshProUGUI PlayerName;
        
        
        public void Accept()
        {
            PlayerManager.Instance.LocalPlayer.Party.JoinParty(characterName);
        }

        public void Decline()
        {
            PlayerManager.Instance.LocalPlayer.Party.DeclineInvite(characterName);
        }
    }
}