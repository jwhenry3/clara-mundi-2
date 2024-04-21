using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi.Quests
{
  public class QuestJournalUI : PlayerUI
  {
    public static QuestJournalUI Instance;

    public QuestInfoUI QuestInfoUI;
    public Transform QuestListContainer;
    public QuestListItemUI QuestListItemPrefab;
    public readonly Dictionary<string, QuestListItemUI> AddedItems = new();

    private void Awake()
    {
      Instance = this;
    }

    protected override void OnPlayerChange(Player _player)
    {
      if (player != null)
      {
        var addedItems = new Dictionary<string, QuestListItemUI>(AddedItems);
        foreach (var kvp in addedItems)
          RemoveQuestById(kvp.Key);
        player.Quests.AcceptedQuests.OnChange -= OnQuestUpdates;
      }
      base.OnPlayerChange(_player);
      if (player == null) return;
      player.Quests.AcceptedQuests.OnChange += OnQuestUpdates;
      LoadQuests();
    }

    private void LoadQuests()
    {
      foreach (Transform child in QuestListContainer)
        Destroy(child.gameObject);
      foreach (string questId in player.Quests.AcceptedQuests)
        AddQuest(questId);
      EventSystem.current.SetSelectedGameObject(AddedItems.First().Value.AutoFocus.gameObject);
    }

    private void AddQuest(string questId)
    {
      if (AddedItems.ContainsKey(questId)) return;
      if (!RepoManager.Instance.QuestRepo.Quests.ContainsKey(questId)) return;
      var listItem = Instantiate(QuestListItemPrefab, QuestListContainer, false);
      listItem.Quest = RepoManager.Instance.QuestRepo.Quests[questId];
      AddedItems.Add(questId, listItem);
      if (AddedItems.First().Value == listItem)
      {
        listItem.AutoFocus.HasFocused = false;
        listItem.AutoFocus.enabled = true;
      }

    }

    private void OnQuestUpdates(SyncListOperation op, int index, string previous, string next, bool asServer)
    {
      if (asServer) return;
      switch (op)
      {
        case SyncListOperation.Add:
          AddQuest(next);
          break;
        case SyncListOperation.RemoveAt:
          RemoveQuestById(previous);
          break;
      }
    }

    private void RemoveQuestById(string questId)
    {
      if (!AddedItems.ContainsKey(questId)) return;
      Destroy(AddedItems[questId]);
      AddedItems.Remove(questId);
    }

    public override void OnDestroy()
    {
      base.OnDestroy();
      var addedItems = new Dictionary<string, QuestListItemUI>(AddedItems);
      foreach (var kvp in addedItems)
        RemoveQuestById(kvp.Key);
    }
  }
}