using System;

namespace ClaraMundi
{
    [Serializable]
    public class CompletedQuest
    {
        public string PlayerName;
        // The completion should not be persisted unless all tasks that are a part
        // of the quest are completed as well. Once the completion is tracked,
        // it cannot be undone. This may or may not prevent additional completions
        // of the same quest depending on Quest configuration (recurring)
        public string QuestId;
        // Should determine whether the player has the ability to accept the
        // quest again if a recurrence is set and the current date is past the
        // recurrence window (next day, next week, etc)
        // we should decide if there should be a separate datetime of weekly
        // and monthly resets in case we want to make quests recur on
        // specific days like monday/wednesday/friday specifically
        public int LastCompletedTimestamp;
    }
}