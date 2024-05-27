﻿using System;
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
    public WindowUI window;
    public QuestInfoUI QuestInfoUI;
    public Transform QuestListContainer;
    public QuestListItemUI QuestListItemPrefab;
    public readonly Dictionary<string, QuestListItemUI> AddedItems = new();

    public CanvasGroupWatcher canvasGroupWatcher;

    private void Awake()
    {
      Instance = this;
    }


    protected override void OnPlayerChange(Player _player)
    {
      foreach (Transform child in QuestListContainer)
        Destroy(child.gameObject);
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
      canvasGroupWatcher.AutoFocusButton = null;
      canvasGroupWatcher.CurrentButton = null;
      foreach (Transform child in QuestListContainer)
        Destroy(child.gameObject);
      if (player == null) return;
      foreach (string questId in player.Quests.AcceptedQuests)
        AddQuest(questId);
    }

    private void AddQuest(string questId)
    {
      if (AddedItems.ContainsKey(questId)) return;
      if (!RepoManager.Instance.QuestRepo.Quests.ContainsKey(questId)) return;
      var listItem = Instantiate(QuestListItemPrefab, QuestListContainer, false);
      listItem.journal = this;
      listItem.Quest = RepoManager.Instance.QuestRepo.Quests[questId];
      AddedItems.Add(questId, listItem);
      if (AddedItems.Count == 1)
      {
        listItem.button.AutoFocus = true;
        QuestInfoUI.Quest = listItem.Quest;
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
      Destroy(AddedItems[questId].gameObject);
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