using System;
using UnityEngine;

namespace ClaraMundi
{
    [Serializable]
    public class DialoguePage
    {
        [Multiline(10)]
        public string Content;
    }
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Clara Mundi/Dialogue/Dialogue")]
    [Serializable]
    public class Dialogue : ScriptableObject
    {
        public string DialogueId = StringUtils.UniqueId();
        [HideInInspector]
        public string AssociatedQuestId;
        public Entity Speaker;
        public DialoguePage[] DialoguePages;
    }
}