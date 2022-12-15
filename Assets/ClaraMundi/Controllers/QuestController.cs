using System;
using System.Collections.Generic;
using FishNet.Object.Synchronizing;

namespace ClaraMundi
{
    public class QuestController : PlayerController
    {
        public readonly SyncList<string> AcceptedQuests = new();
        public readonly SyncDictionary<string, CompletedQuest> QuestCompletions = new();
        public readonly SyncDictionary<string, QuestTaskProgress> TaskProgress = new();

        private QuestRepo repo => RepoManager.Instance.QuestRepo;
        

        protected override void Awake()
        {
            base.Awake();
            player.Inventory.ItemStorage.PrivateItems.OnChange += OnItemChange;
            // Listen for changes to inventory and events in the game that should trigger task progress changes
        }

        private void OnDestroy()
        {
            // destroy the listening
        }

        private void OnItemChange(SyncDictionaryOperation op, string key, ItemInstance itemInstance, bool asServer)
        {
            foreach (var questId in AcceptedQuests)
            {
                // do not track task progress for completed quests
                if (QuestCompletions.ContainsKey(questId)) continue;
                var quest = repo.Quests[questId];
                if (!quest.ItemTasksByItemId.ContainsKey(itemInstance.ItemId)) continue;
                var quantity = player.Inventory.ItemStorage.QuantityOf(itemInstance.ItemId);
                var remaining = quantity;
                bool changed = false;
                foreach (var task in quest.ItemTasksByItemId[itemInstance.ItemId])
                {
                   
                    var progress = new QuestTaskProgress()
                    {
                        QuestId = task.QuestId,
                        QuestTaskId = task.QuestTaskId,
                        PlayerName = player.Entity.entityName,
                        QuantityComplete = remaining > task.ItemQuantity ? task.ItemQuantity : remaining,
                        IsVolatile = true
                    };
                    remaining = remaining - task.ItemQuantity;
                    TaskProgress[task.QuestTaskId] = progress;
                    if (remaining <= 0)
                        remaining = 0;
                    changed = true;
                }

                if (changed)
                    CheckQuestProgress(quest);
            }
        }

        private void CheckQuestProgress(Quest quest)
        {
            // check TaskProgress of each task and make sure all are completed fully
            // if so, create a quest completion for it and consume all items in the inventory
            // that have not been given to the proper NPCs
        }
        public void AcceptQuest(string questId)
        {
            if (!RepoManager.Instance.QuestRepo.Quests.ContainsKey(questId)) return;
            if (AcceptedQuests.Contains(questId)) return;
            AcceptedQuests.Add(questId);
            
        }

        public void AbandonQuest(string questId)
        {
            
        }

    }
}