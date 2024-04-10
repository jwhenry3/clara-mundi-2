using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClaraMundi
{
    [CreateAssetMenu(fileName = "QuestRepo", menuName = "Clara Mundi/Quest/QuestRepo")]
    [Serializable]
    public class QuestRepo : ScriptableObject
    {
        public Dictionary<string, Quest> Quests = new();
        public Dictionary<string, List<Quest>> QuestsByStarterEntityId = new();
        public Dictionary<string, List<QuestTask>> TasksByEntityId = new();
        public Dictionary<string, List<QuestTask>> TasksByEntityTypeId = new();
        

        public Quest[] QuestList;

        public Dictionary<string, List<Quest>> NextQuests = new();

        public Quest GetQuest(string questId)
        {
            return Quests.ContainsKey(questId) ? Quests[questId] : null;
        }
        
        public void OnEnable()
        {
            if (QuestList == null) return;
            foreach (var quest in QuestList)
            {
                Quests.Add(quest.QuestId, quest);
                quest.Initialize();
                LoadTasksByEntities(quest);
                if (quest.Requirement == null) continue;
                    LoadNextQuestsFor(quest, quest.Requirement);
            }
        }

        private void LoadTasksByEntities(Quest quest)
        {
            // Provides a quick way for an entity to determine if it has quests
            if (quest.Starter != null && quest.Starter.Speaker != null)
            {
                if (!QuestsByStarterEntityId.ContainsKey(quest.Starter.Speaker.entityId.Value))
                    QuestsByStarterEntityId[quest.Starter.Speaker.entityId.Value] = new();
                if (!QuestsByStarterEntityId[quest.Starter.Speaker.entityId.Value].Contains(quest))
                    QuestsByStarterEntityId[quest.Starter.Speaker.entityId.Value].Add(quest);
            }

            // Provides a quick way for an entity to determine what dialogues, tasks, etc
            // that an entity is responsible for and can update the necessary parts remotely
            // this reduces need for iteration through the master quest list
            // and allows more efficient updates
            foreach (var task in quest.Tasks)
            {
                switch (task.Type)
                {
                    case QuestTaskType.Dialogue:
                        if (quest.Starter != null && quest.Starter.Speaker != null)
                            AddTaskTo(task.Dialogue.Speaker.entityId.Value, TasksByEntityId, task);
                        break;
                    case QuestTaskType.Gather:
                        if (task.GiveItemDialogue != null && task.GiveItemDialogue.Speaker!= null)
                            AddTaskTo(task.GiveItemDialogue.Speaker.entityId.Value, TasksByEntityId, task);
                        break;
                    case QuestTaskType.Dispatch:
                        if (task.DispatchEntityType != null)
                            AddTaskTo(task.DispatchEntityType.EntityTypeId, TasksByEntityTypeId, task);
                        break;
                }
            }
        }

        private void AddTaskTo(string key, Dictionary<string, List<QuestTask>> collection, QuestTask task)
        {
            if (!collection.ContainsKey(key))
                collection[key] = new();
            if (!collection[key].Contains(task))
                collection[key].Add(task);
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