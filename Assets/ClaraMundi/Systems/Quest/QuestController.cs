using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace ClaraMundi
{
    public class QuestController : PlayerController
    {
        [SyncVar(ReadPermissions = ReadPermission.OwnerOnly, OnChange = nameof(OnDialogueComplete))]
        public string LastCompletedDialogueId;
        [SyncVar(ReadPermissions = ReadPermission.OwnerOnly, OnChange = nameof(OnEntityTypeDispatched))]
        public string LastEntityTypeDispatched;
        [SyncObject(ReadPermissions = ReadPermission.OwnerOnly)]
        public readonly SyncList<string> AcceptedQuests = new();
        [SyncObject(ReadPermissions = ReadPermission.OwnerOnly)]
        public readonly SyncDictionary<string, CompletedQuest> QuestCompletions = new();
        [SyncObject(ReadPermissions = ReadPermission.OwnerOnly)]
        public readonly SyncDictionary<string, QuestTaskProgress> TaskProgress = new();

        private QuestRepo repo => RepoManager.Instance.QuestRepo;
        private ItemRepo itemRepo => RepoManager.Instance.ItemRepo;
        public List<Quest> StartingQuests = new();

        public override void OnStartServer()
        {
            base.OnStartServer();
            foreach (var quest in StartingQuests)
                AcceptedQuests.Add(quest.QuestId);
            player.Inventory.ItemStorage.PrivateItems.OnChange += OnItemChange;
        }

        private void OnDestroy()
        {
            // destroy the listening
            if (IsServer)
                player.Inventory.ItemStorage.PrivateItems.OnChange -= OnItemChange;
        }

        private void OnItemChange(SyncDictionaryOperation op, string key, ItemInstance itemInstance, bool asServer)
        {
            if (!asServer) return;
            if (itemInstance == null) return;
            Debug.Log("Update Quests!");
            foreach (var questId in AcceptedQuests)
            {
                // do not track task progress for completed quests
                if (QuestCompletions.ContainsKey(questId)) continue;
                var quest = repo.Quests[questId];
                Debug.Log("Quest Found!");
                if (!quest.ItemTasksByItemId.ContainsKey(itemInstance.ItemId)) continue;
                Debug.Log("Update Quest: " + quest.Title);
                if (UpdateItemTasksFor(quest, itemInstance))
                    CheckQuestProgress(quest);
            }
        }

        private void OnDialogueComplete(string previous, string next, bool asServer)
        {
            if (!asServer) return;
            foreach (var questId in AcceptedQuests)
            {
                // do not track task progress for completed quests
                if (QuestCompletions.ContainsKey(questId)) continue;
                var quest = repo.Quests[questId];
                bool dialogueProgressed = CheckDialogueProgress(quest, next);
                bool turnInProgressed = CheckItemTurnInProgress(quest, next);
                if (dialogueProgressed || turnInProgressed)
                    CheckQuestProgress(quest);
            }
        }

        private void OnEntityTypeDispatched(string previous, string next, bool asServer)
        {
            if (!asServer) return;
            if (next == null) return;
            foreach (var questId in AcceptedQuests)
            {
                // do not track task progress for completed quests
                if (QuestCompletions.ContainsKey(questId)) continue;
                var quest = repo.Quests[questId];
                if (CheckDispatchProgress(quest, next))
                    CheckQuestProgress(quest);
            }
        }

        private bool CheckDispatchProgress(Quest quest, string entityTypeId)
        {
            if (!quest.DispatchTasksByEntityTypeId.ContainsKey(entityTypeId)) return false;
            bool updated = false;
            foreach (var task in quest.DispatchTasksByEntityTypeId[entityTypeId])
            {
                var previousProgress =
                    TaskProgress.ContainsKey(task.QuestTaskId) ? TaskProgress[task.QuestTaskId] : null;
                // do not process complete tasks
                if (previousProgress is { IsComplete: true }) return false;
                // set up the current dispatch count
                int count = Math.Min(previousProgress != null ? previousProgress.DispatchCount + 1 : 1, task.DispatchQuantity);
                var progress = new QuestTaskProgress()
                {
                    QuestId = task.QuestId,
                    QuestTaskId = task.QuestTaskId,
                    PlayerName = player.Entity.entityName,
                    DispatchCount = count,
                    IsComplete = count == task.DispatchQuantity
                };
                TaskProgress[task.QuestTaskId] = progress;
                if (previousProgress == null || previousProgress.IsComplete != progress.IsComplete)
                    updated = true;
            }

            return updated;
        }

        private bool CheckItemTurnInProgress(Quest quest, string dialogueId)
        {
            if (!quest.ItemTasksByDialogueId.ContainsKey(dialogueId)) return false;
            if (quest.ItemTasksByDialogueId[dialogueId].Count == 0) return false;
            bool changed = false;
            foreach (var task in quest.ItemTasksByDialogueId[dialogueId])
            {
                // update item quantities for gathering quests for the item in this task
                // that way we can make sure the state is updated before we confirm
                // completion
                if (!TaskProgress.ContainsKey(task.QuestTaskId))
                    UpdateItemTasksFor(quest, task.GatherItem);
                var previousProgress = TaskProgress[task.QuestTaskId];
                // if somehow the task was already marked complete
                // skip the extra processing
                if (previousProgress.IsComplete) continue;
                // remove items from inventory since the player is turning in the items requested
                // maybe in the future, allow a player to specify a specific item instance to turn in
                // this would be useful if the player has the same item but with different stats
                var couldRemove = player.Inventory.ItemStorage.RemoveItem(task.GatherItem.ItemId, task.ItemQuantity);
                if (!couldRemove)
                    return changed;
                var progress = new QuestTaskProgress()
                {
                    QuestId = task.QuestId,
                    QuestTaskId = task.QuestTaskId,
                    PlayerName = player.Entity.entityName,
                    ItemsHeld = previousProgress.ItemsHeld,
                    ItemsTurnedIn =  true,
                    IsComplete =  true,
                    IsVolatile = false
                };
                TaskProgress[task.QuestTaskId] = progress;
                changed = true;
            }

            return changed;
        }
        private bool CheckDialogueProgress(Quest quest, string dialogueId)
        {
            if (!quest.DialogueTasksByDialogueId.ContainsKey(dialogueId)) return false;
            if (quest.DialogueTasksByDialogueId[dialogueId].Count == 0) return false;
            var dialogue = quest.DialogueTasksByDialogueId[dialogueId].First().Dialogue;
            bool updated = false;
            foreach (var task in quest.DialogueTasksByDialogueId[dialogue.DialogueId])
            {
                var previousProgress =
                    TaskProgress.ContainsKey(task.QuestTaskId) ? TaskProgress[task.QuestTaskId] : null;
                var progress = new QuestTaskProgress()
                {
                    QuestId = task.QuestId,
                    QuestTaskId = task.QuestTaskId,
                    PlayerName = player.Entity.entityName,
                    IsVolatile = false,
                    DialogueCompleted = true,
                    IsComplete = true
                };
                TaskProgress[task.QuestTaskId] = progress;
                if (previousProgress == null || previousProgress.IsComplete != progress.IsComplete)
                    updated = true;
            }

            return updated;
        }
        private bool UpdateItemTasksFor(Quest quest, ItemInstance itemInstance)
        {
            return itemInstance != null && UpdateItemTasksFor(quest, itemRepo.GetItem(itemInstance.ItemId));
        }

        private bool UpdateItemTasksFor(Quest quest, Item item)
        {
            if (item == null) return false;
            var quantity = player.Inventory.ItemStorage.QuantityOf(item.ItemId);
            var remaining = quantity;
            bool changed = false;
            foreach (var task in quest.ItemTasksByItemId[item.ItemId])
            {
                var previousProgress =
                    TaskProgress.ContainsKey(task.QuestTaskId) ? TaskProgress[task.QuestTaskId] : null;
                if (previousProgress != null && (previousProgress.IsComplete || previousProgress.ItemsTurnedIn)) continue;
                Debug.Log("Update Progress: " + task.ShortDescription);
                var progress = new QuestTaskProgress()
                {
                    QuestId = task.QuestId,
                    QuestTaskId = task.QuestTaskId,
                    PlayerName = player.Entity.entityName,
                    ItemsHeld = remaining > task.ItemQuantity ? task.ItemQuantity : remaining,
                    IsVolatile = true
                };
                
                remaining -= task.ItemQuantity;
                TaskProgress[task.QuestTaskId] = progress;
                if (remaining <= 0)
                    remaining = 0;
                changed = true;
            }

            return changed;
        }

        private void CheckQuestProgress(Quest quest)
        {
            // check TaskProgress of each task and make sure all are completed fully
            // if so, create a quest completion for it and consume all items in the inventory
            // that have not been given to the proper NPCs
            List<string> completed = new();
            // return early if any tasks are incomplete
            // avoid extra processing when we do not need it
            foreach (var task in quest.Tasks)
            {
                // there is no progress for this task
                if (!TaskProgress.ContainsKey(task.QuestTaskId)) return;
                // the quest is not complete
                if (!TaskProgress[task.QuestTaskId].IsComplete) return;
                // the quest is complete
                completed.Add(task.QuestTaskId);
            }

            if (completed.Count != quest.Tasks.Length) return;
            // quest is complete, record new completion
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;
            var completion = new CompletedQuest()
            {
                QuestId = quest.QuestId,
                PlayerName = player.Entity.entityName,
                LastCompletedTimestamp = secondsSinceEpoch
            };
            QuestCompletions[quest.QuestId] = completion;
            // we no longer need to track the completed quest
            // we can refer to the QuestCompletions for that information
            AcceptedQuests.Remove(quest.QuestId);
            // remove task progress since we no longer track it after a completion
            foreach (string id in completed)
                TaskProgress.Remove(id);

        }

        public bool HasRequiredItems(Quest quest)
        {
            return quest.Requirement.RequiredItemsHeld.All(requiredItem => player.Inventory.ItemStorage.HeldItemIds.Contains(requiredItem.ItemId));
        }

        public bool HasRequiredCompletions(Quest quest)
        {
            return quest.Requirement.PrecedingQuests.All(requiredQuestCompletion =>
                QuestCompletions.ContainsKey(requiredQuestCompletion.QuestId));
        }

        public bool IsRequiredLevel(Quest quest)
        {
            return player.Stats.Level >= quest.Requirement.RequiredLevel;
        }
        public bool CanAcceptQuest(string questId)
        {
            if (!repo.Quests.ContainsKey(questId)) return false;
            if (AcceptedQuests.Contains(questId)) return false;
            var quest = repo.Quests[questId];
            return IsRequiredLevel(quest) && HasRequiredCompletions(quest) && HasRequiredItems(quest);
        }
        public void AcceptQuest(string questId)
        {
            if (!repo.Quests.ContainsKey(questId)) return;
            if (AcceptedQuests.Contains(questId)) return;
            var quest = repo.Quests[questId];
            // report the level is not met
            if (player.Stats.Level < quest.Requirement.RequiredLevel) return;
            
            if (quest.Requirement.PrecedingQuests.Any(requiredQuestCompletion => !QuestCompletions.ContainsKey(requiredQuestCompletion.QuestId)))
                return;
            if (quest.Requirement.RequiredItemsHeld.Any(requiredItem => player.Inventory.ItemStorage.GetInstanceByItemId(requiredItem.ItemId) == null))
                return;
            // requirements are met, accept the quest
            AcceptedQuests.Add(questId);
            
        }

        public void AbandonQuest(string questId)
        {
            if (!AcceptedQuests.Contains(questId)) return;
            var quest = repo.Quests[questId];
            foreach (var task in quest.Tasks)
            {
                if (TaskProgress.ContainsKey(task.QuestTaskId))
                    TaskProgress.Remove(task.QuestTaskId);
            }

            AcceptedQuests.Remove(questId);
        }

    }
}