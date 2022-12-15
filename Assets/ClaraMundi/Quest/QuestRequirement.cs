using System;

namespace ClaraMundi
{
    [Serializable]
    public class QuestRequirement
    {
        public int RequiredLevel = 1;
        // Required quests that must be completed just before this is
        // available. You only need to set the most recent previous quest
        // as the quest chain is recursive
        public Quest[] PrecedingQuests;
        // Most likely key items
        public Item[] RequiredItemsHeld;
        // more TBD as new systems are implemented
        // EX: guild rank, faction standing, etc
    }
}