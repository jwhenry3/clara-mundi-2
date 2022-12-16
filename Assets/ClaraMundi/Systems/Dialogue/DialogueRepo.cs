using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClaraMundi
{
    [CreateAssetMenu(fileName = "DialogueRepo", menuName = "Clara Mundi/Dialogue/DialogueRepo")]
    [Serializable]
    public class DialogueRepo : ScriptableObject
    {
        public Dictionary<string, Dialogue> Dialogues;

        public Dialogue[] DialogueList;

        public void OnEnable()
        {
            Dialogues = new();
            if (DialogueList == null) return;
            foreach (var dialogue in DialogueList)
            {
                Dialogues.Add(dialogue.DialogueId, dialogue);
            }
        }
        
    }
}