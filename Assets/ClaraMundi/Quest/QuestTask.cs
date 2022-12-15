using System;
using Sirenix.OdinInspector;

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
        [BoxGroup("Identity")]
        public string QuestTaskId = Guid.NewGuid().ToString();
        [BoxGroup("Identity")]
        public QuestTaskType Type = QuestTaskType.Dialogue;
        [TabGroup("Gather")]
        public Item GatherItem;
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