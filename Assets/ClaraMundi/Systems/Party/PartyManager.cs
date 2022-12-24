using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ClaraMundi
{
    public class PartyManager : MonoBehaviour
    {
        public readonly Dictionary<string, PartyModel> PartiesByLeader = new();
        public static PartyManager Instance;
        private void Awake()
        {
            Instance = this;
        }


        PartyModel GetPlayerParty(string characterName)
        {
            return PartiesByLeader.FirstOrDefault(kvp => kvp.Value.Members.Contains(characterName)).Value;
        }

        public void ServerSendMessage(ChatMessage message)
        {
            var party = GetPlayerParty(message.SenderCharacterName);
            if (party == null) return;
        }
        

    }
}