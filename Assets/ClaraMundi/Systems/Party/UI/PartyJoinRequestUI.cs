using System;
using TMPro;
using UnityEngine;

namespace ClaraMundi
{
    public class PartyJoinRequestUI : MonoBehaviour
    {
        public string joiningPlayerId;
        public TextMeshProUGUI PlayerName;
        

        public void Accept()
        {
            PlayerManager.Instance.LocalPlayer.Party.AcceptRequest(joiningPlayerId);
        }

        public void Decline()
        {
            PlayerManager.Instance.LocalPlayer.Party.DeclineRequest(joiningPlayerId);
        }
    }
}