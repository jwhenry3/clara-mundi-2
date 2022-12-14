using TMPro;
using UnityEngine;

namespace ClaraMundi
{
    public class PartyInviteRequestUI : MonoBehaviour
    {
        public string invitingPlayerId;
        public TextMeshProUGUI PlayerName;
        
        
        public void Accept()
        {
            PlayerManager.Instance.LocalPlayer.Party.JoinParty(invitingPlayerId);
        }

        public void Decline()
        {
            PlayerManager.Instance.LocalPlayer.Party.DeclineInvite(invitingPlayerId);
        }
    }
}