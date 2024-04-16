using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace ClaraMundi
{
  public class QuestController : PlayerController
  {
    public readonly SyncVar<string> LastCompletedDialogueId = new(new SyncTypeSettings(ReadPermission.OwnerOnly));
    public readonly SyncVar<string> LastEntityTypeDispatched = new(new SyncTypeSettings(ReadPermission.OwnerOnly));
    public readonly SyncList<string> AcceptedQuests = new(new SyncTypeSettings(ReadPermission.OwnerOnly));
    public readonly SyncList<string> TrackedQuests = new(new SyncTypeSettings(ReadPermission.OwnerOnly));
    public readonly SyncDictionary<string, CompletedQuest> QuestCompletions = new(new SyncTypeSettings(ReadPermission.OwnerOnly));
    public readonly SyncDictionary<string, QuestTaskProgress> TaskProgress = new(new SyncTypeSettings(ReadPermission.OwnerOnly));

    private QuestRepo repo => RepoManager.Instance.QuestRepo;
    private ItemRepo itemRepo => RepoManager.Instance.ItemRepo;
    public List<Quest> StartingQuests = new();


    [ServerRpc]
    public void AcceptQuest(string questId) => ServerAcceptQuest(questId);
    [ServerRpc]
    public void AbandonQuest(string questId) => ServerAbandonQuest(questId);
    [ServerRpc]
    public void TrackQuest(string questId) => ServerTrackProgress(questId);
    [ServerRpc]
    public void UntrackQuest(string questId) => ServerUntrackQuest(questId);

    public void ToggleTrack(string questId, bool value)
    {
      if (value)
        TrackQuest(questId);
      else
        UntrackQuest(questId);
    }

    public QuestTaskProgress GetTaskProgress(string taskId) => TaskProgress.ContainsKey(taskId) ? TaskProgress[taskId] : null;

    public CompletedQuest GetCompletion(string questId) =>
        QuestCompletions.ContainsKey(questId) ? QuestCompletions[questId] : null;

    void OnEnable()
    {
      LastCompletedDialogueId.OnChange += OnDialogueComplete;
      LastEntityTypeDispatched.OnChange += OnEntityTypeDispatched;
    }
    void OnDisable()
    {
      LastCompletedDialogueId.OnChange -= OnDialogueComplete;
      LastEntityTypeDispatched.OnChange -= OnEntityTypeDispatched;
    }

    public override void OnStartServer()
    {
      base.OnStartServer();
      foreach (var quest in StartingQuests)
      {
        AcceptedQuests.Add(quest.QuestId);
        UpdateItemProgressFor(quest.QuestId);
      }

      player.Inventory.ItemStorage.Items.OnChange += OnItemChange;
    }

    private void OnDestroy()
    {
      // destroy the listening
      if (IsServerStarted)
        player.Inventory.ItemStorage.Items.OnChange -= OnItemChange;
    }

    private void UpdateItemProgressFor(string questId)
    {
      if (!AcceptedQuests.Contains(questId)) return;
      var quest = repo.Quests[questId];
      List<Item> items = new();
      foreach (var task in quest.Tasks)
      {
        if (task.Type != QuestTaskType.Gather) continue;
        if (task.GatherItem == null || items.Contains(task.GatherItem)) continue;
        items.Add(task.GatherItem);
        UpdateItemTasksFor(quest, task.GatherItem);
      }
    }

    private void OnItemChange(SyncDictionaryOperation op, int key, ItemInstance itemInstance, bool asServer)
    {
      if (!asServer) return;
      if (itemInstance == null) return;
      foreach (string questId in AcceptedQuests)
      {
        // do not track task progress for completed quests
        if (QuestCompletions.ContainsKey(questId)) continue;
        var quest = repo.Quests[questId];
        if (!quest.ItemTasksByItemId.ContainsKey(itemInstance.ItemId)) continue;
        if (UpdateItemTasksFor(quest, itemInstance))
          CheckQuestProgress(quest);
      }
    }

    private void OnDialogueComplete(string previous, string next, bool asServer)
    {
      if (!asServer) return;
      foreach (string questId in AcceptedQuests)
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
      foreach (string questId in AcceptedQuests)
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
        var previousProgress = GetTaskProgress(task.QuestTaskId);
        // do not process complete tasks
        if (previousProgress is { IsComplete: true }) return false;
        // set up the current dispatch count
        int count = Math.Min((previousProgress?.DispatchCount ?? 0) + 1, task.DispatchQuantity);
        var progress = new QuestTaskProgress()
        {
          QuestId = task.QuestId,
          QuestTaskId = task.QuestTaskId,
          PlayerName = player.Entity.entityName.Value,
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
          PlayerName = player.Entity.entityName.Value,
          ItemsHeld = previousProgress.ItemsHeld,
          ItemsTurnedIn = true,
          IsComplete = true,
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
          PlayerName = player.Entity.entityName.Value,
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
    private bool UpdateItemTasksFor(Quest quest, ItemInstance itemInstance) =>
        itemInstance != null && UpdateItemTasksFor(quest, itemRepo.GetItem(itemInstance.ItemId));


    private bool UpdateItemTasksFor(Quest quest, Item item)
    {
      if (item == null) return false;
      var quantity = player.Inventory.ItemStorage.QuantityOf(item.ItemId);
      var remaining = quantity;
      bool changed = false;
      foreach (var task in quest.ItemTasksByItemId[item.ItemId])
      {
        var previousProgress = GetTaskProgress(task.QuestTaskId);
        // task is already complete
        if (previousProgress is { IsComplete: true }) continue;
        var progress = new QuestTaskProgress()
        {
          QuestId = task.QuestId,
          QuestTaskId = task.QuestTaskId,
          PlayerName = player.Entity.entityName.Value,
          ItemsHeld = remaining > task.ItemQuantity ? task.ItemQuantity : remaining,
          IsVolatile = true
        };

        TaskProgress[task.QuestTaskId] = progress;
        changed = true;

        remaining -= task.ItemQuantity;
        if (remaining <= 0)
          remaining = 0;
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
        if (GetTaskProgress(task.QuestTaskId) is { IsComplete: true })
          completed.Add(task.QuestTaskId);
      }

      if (completed.Count != quest.Tasks.Length) return;
      // quest is complete, record new completion
      TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
      int secondsSinceEpoch = (int)t.TotalSeconds;
      var completion = new CompletedQuest()
      {
        QuestId = quest.QuestId,
        PlayerName = player.Entity.entityName.Value,
        LastCompletedTimestamp = secondsSinceEpoch
      };
      QuestCompletions[quest.QuestId] = completion;
      // we no longer need to track the completed quest
      // we can refer to the QuestCompletions for that information
      AcceptedQuests.Remove(quest.QuestId);
      // remove task progress since we no longer track it after a completion
      foreach (string id in completed)
        TaskProgress.Remove(id);
      ServerUntrackQuest(quest.QuestId);
    }

    public bool HasRequiredItems(Quest quest) =>
        quest.Requirement.RequiredItemsHeld.All(requiredItem => player.Inventory.ItemStorage.HeldItemIds.Contains(requiredItem.ItemId));

    public bool HasRequiredCompletions(Quest quest) =>
        quest.Requirement.PrecedingQuests.All(requiredQuestCompletion => QuestCompletions.ContainsKey(requiredQuestCompletion.QuestId));

    public bool IsRequiredLevel(Quest quest) => player.Stats.Level.Value >= quest.Requirement.RequiredLevel;

    public bool CanAcceptQuest(string questId)
    {
      if (!repo.Quests.ContainsKey(questId)) return false;
      if (AcceptedQuests.Contains(questId)) return false;
      var quest = repo.Quests[questId];
      return IsRequiredLevel(quest) && HasRequiredCompletions(quest) && HasRequiredItems(quest);
    }

    public void ServerAcceptQuest(string questId)
    {
      if (!IsServerStarted) return;
      if (!repo.Quests.ContainsKey(questId)) return;
      if (AcceptedQuests.Contains(questId)) return;
      if (!CanAcceptQuest(questId)) return;
      // requirements are met, accept the quest
      AcceptedQuests.Add(questId);
      ServerTrackProgress(questId);
    }

    public void ServerAbandonQuest(string questId)
    {
      if (!AcceptedQuests.Contains(questId)) return;
      var quest = repo.Quests[questId];
      foreach (var task in quest.Tasks)
        TaskProgress.Remove(task.QuestTaskId);

      AcceptedQuests.Remove(questId);
      ServerUntrackQuest(questId);
    }

    public void ServerTrackProgress(string questId)
    {
      if (!AcceptedQuests.Contains(questId)) return;
      if (QuestCompletions.ContainsKey(questId)) return;
      if (!TrackedQuests.Contains(questId))
        TrackedQuests.Add(questId);
    }

    public void ServerUntrackQuest(string questId)
    {
      TrackedQuests.Remove(questId);
    }

  }
}