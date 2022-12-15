using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClaraMundi
{
    [CreateAssetMenu(fileName = "QuestRepo", menuName = "Clara Mundi/Quest/QuestRepo")]
    [Serializable]
    public class QuestRepo : ScriptableObject
    {
        public Dictionary<string, Quest> Quests;

        public Quest[] QuestList;

        public Dictionary<string, List<Quest>> NextQuests = new();

        public Quest GetQuest(string questId)
        {
            return Quests.ContainsKey(questId) ? Quests[questId] : null;
        }
        
        public void OnEnable()
        {
            Quests = new();
            NextQuests = new();
            if (QuestList == null) return;
            foreach (var quest in QuestList)
            {
                Quests.Add(quest.QuestId, quest);
                
                if (quest.Requirement == null) continue;
                    LoadNextQuestsFor(quest, quest.Requirement);
            }
        }

        private void LoadNextQuestsFor(Quest quest, QuestRequirement requirement)
        {
            if (requirement.PrecedingQuests.Length <= 0) return;
            foreach (var requiredQuest in requirement.PrecedingQuests)
                GetOrCreateNextQuestList(requiredQuest.QuestId).Add(quest);
        }
        public List<Quest> GetOrCreateNextQuestList(string questId)
        {
            if (!NextQuests.ContainsKey(questId))
                NextQuests[questId] = new();
            return NextQuests[questId];
        }

        
    }
}