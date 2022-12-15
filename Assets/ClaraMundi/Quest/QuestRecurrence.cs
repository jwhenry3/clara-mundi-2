using System;

namespace ClaraMundi
{
    public enum QuestRecurrenceType
    {
        None,
        Daily,
        Weekly,
        Monthly
    }
    [Serializable]
    public class QuestRecurrence
    {
        public bool IsRecurring;
        public QuestRecurrenceType Type;
    }
}