using System;
using System.Collections.Generic;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace ClaraMundi.Quests
{
  public class QuestTrackerUI : PlayerUI
  {
    public static QuestTrackerUI Instance;

    public MoveSibling MoveSibling;
    public Transform QuestListContainer;
    public QuestListItemUI QuestListItemPrefab;
    readonly Dictionary<string, QuestListItemUI> TrackedQuestListItems = new();

    public CanvasGroup CanvasGroup;

    public override void Start()
    {
      base.Start();
      MoveSibling = GetComponent<MoveSibling>();
      Instance = this;
    }
    protected override void OnPlayerChange(Player _player)
    {
      if (player != null)
      {
        player.Quests.TrackedQuests.OnChange -= OnTrackedChange;
        ClearTrackedQuests();
      }

      base.OnPlayerChange(_player);
      if (player == null) return;
      player.Quests.TrackedQuests.OnChange += OnTrackedChange;
      LoadTrackedQuests();
    }

    public override void OnDestroy()
    {
      base.OnDestroy();
      if (player == null) return;
      player.Quests.TrackedQuests.OnChange -= OnTrackedChange;
      foreach (string questId in player.Quests.TrackedQuests)
        AddTrackedQuest(questId);
    }

    private void OnTrackedChange(SyncListOperation op, int index, string previous, string next, bool asServer)
    {
      if (asServer) return;
      LoadTrackedQuests();
    }

    private void LoadTrackedQuests()
    {
      ClearTrackedQuests();
      for (var i = 0; i < 6; i++)
      {
        if (player.Quests.TrackedQuests.Count > i)
          AddTrackedQuest(player.Quests.TrackedQuests[i]);
      }
    }

    private void AddTrackedQuest(string questId)
    {
      if (TrackedQuestListItems.ContainsKey(questId)) return;
      var instance = Instantiate(QuestListItemPrefab, QuestListContainer, false);
      instance.IsQuestTackerQuest = true;
      instance.Quest = RepoManager.Instance.QuestRepo.Quests[questId];
      TrackedQuestListItems.Add(questId, instance);
    }

    private void AddTrackedQuestAtIndex(int index, string questId)
    {
      if (!TrackedQuestListItems.ContainsKey(questId))
        AddTrackedQuest(questId);
      TrackedQuestListItems[questId].transform.SetSiblingIndex(index);

    }

    private void RemoveTrackedQuest(string questId)
    {
      if (!TrackedQuestListItems.ContainsKey(questId)) return;
      var instance = TrackedQuestListItems[questId];
      TrackedQuestListItems.Remove(questId);
      Destroy(instance.gameObject);
    }

    private void ClearTrackedQuests()
    {
      foreach (var kvp in TrackedQuestListItems)
        Destroy(kvp.Value.gameObject);
      TrackedQuestListItems.Clear();
    }
  }
}