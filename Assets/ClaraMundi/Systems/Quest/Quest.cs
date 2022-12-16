using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace ClaraMundi
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Clara Mundi/Quest/Quest")]
    [Serializable]
    public class Quest : ScriptableObject
    {
        public string QuestId = Guid.NewGuid().ToString();
        [BoxGroup("Identity")]
        public string Title = "";
        [BoxGroup("Identity")]
        public string ShortDescription = "";
        [BoxGroup("Identity")]
        public Dialogue Starter;
        [BoxGroup("Identity")]
        [Multiline(10)]
        public string Lore = "";
        [BoxGroup("Requirements")]
        public QuestRequirement Requirement;
        [TabGroup("Tasks")]
        public QuestTask[] Tasks;
        [TabGroup("Rewards")]
        public LootItem[] Rewards;
        [TabGroup("Rewards")]
        public int CurrencyReward = 0;
        // More reward types can be added as systems are implemented
        // IE: faction standing, guild experience, etc

        // Immediate quests that follow this quest
        public List<Quest> NextQuests => RepoManager.Instance.QuestRepo.GetOrCreateNextQuestList(QuestId);

        public Dictionary<string, List<QuestTask>> ItemTasksByItemId = new();
        public Dictionary<string, List<QuestTask>> ItemTasksByDialogueId = new();
        public Dictionary<string, List<QuestTask>> DialogueTasksByDialogueId = new();
        public Dictionary<string, List<QuestTask>> DispatchTasksByEntityTypeId = new();

        public void Initialize()
        {
            if (Starter != null)
                Starter.AssociatedQuestId = QuestId;
            UpdateTaskList();
        }
        private void UpdateTaskList()
        {
            if (Tasks == null) return;
            foreach (var task in Tasks)
            {
                task.QuestId = QuestId;
                var list = ItemTasksByItemId;
                var id = task.GatherItem.ItemId;
                switch (task.Type)
                {
                    case QuestTaskType.Gather:
                    {
                        if (task.GiveItemDialogue == null) continue;
                        if (!ItemTasksByDialogueId.ContainsKey(task.GiveItemDialogue.DialogueId))
                            ItemTasksByDialogueId[task.GiveItemDialogue.DialogueId] = new();
                        ItemTasksByDialogueId[task.GiveItemDialogue.DialogueId].Add(task);
                        break;
                    }
                    case QuestTaskType.Dialogue:
                    {
                        if (task.Dialogue == null) continue;
                        list = DialogueTasksByDialogueId;
                        id = task.Dialogue.DialogueId;
                        break;
                    }
                    case QuestTaskType.Dispatch:
                    {
                        if (task.DispatchEntityType == null) continue;
                        list = DispatchTasksByEntityTypeId;
                        id = task.DispatchEntityType.EntityTypeId;
                        break;
                    }
                    default:
                        continue;
                }
                if (!list.ContainsKey(id))
                    list[id] = new();
                list[id].Add(task);
            }
        }
    }
}