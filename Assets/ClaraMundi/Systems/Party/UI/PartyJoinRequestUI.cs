using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace ClaraMundi
{
    public class PartyJoinRequestUI : MonoBehaviour
    {
        public string characterName;
        public TextMeshProUGUI PlayerName;
        

        public void Accept()
        {
            PlayerManager.Instance.LocalPlayer.Party.AcceptRequest(characterName);
        }

        public void Decline()
        {
            PlayerManager.Instance.LocalPlayer.Party.DeclineRequest(characterName);
        }
    }
}