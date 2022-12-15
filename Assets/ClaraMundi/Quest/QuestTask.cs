using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ClaraMundi
{
    public enum QuestTaskType
    {
        Gather,
        Dialogue,
        Dispatch,
    }
    [Serializable]
    public class QuestTask
    {
        [HideInInspector]
        public string QuestId;
        [BoxGroup("Identity")]
        public string QuestTaskId = Guid.NewGuid().ToString();
        [BoxGroup("Identity")]
        public QuestTaskType Type = QuestTaskType.Dialogue;
        [BoxGroup("Identity")]
        [Multiline(2)]
        public string ShortDescription;
        [BoxGroup("Identity")]
        [Multiline(4)]
        public string LongDescription;
        [TabGroup("Gather")]
        public Item GatherItem;
        [TabGroup("Gather")]
        public Dialogue GiveItemDialogue;
        [TabGroup("Gather")]
        public int ItemQuantity = 1;
        [TabGroup("Dialogue")]
        public Dialogue Dialogue;
        [TabGroup("Dispatch")]
        public EntityType DispatchEntityType;
        [TabGroup("Dispatch")]
        public int DispatchQuantity = 1;
    }
}