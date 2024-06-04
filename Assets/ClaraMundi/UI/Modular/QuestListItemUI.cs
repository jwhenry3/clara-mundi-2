
using System;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace ClaraMundi.Quests
{
  public class QuestListItemUI : PlayerUI, IPointerDownHandler
  {
    public Quest Quest
    {
      get => _quest;
      set => SetQuest(value);
    }
    public QuestJournalUI journal;
    public ButtonUI button;
    public TextMeshProUGUI QuestTitle;
    public TextMeshProUGUI QuestLevel;
    public TextMeshProUGUI QuestDescription;
    public GameObject TrackedStatusContainer;
    public Toggle TrackedStatusToggle;

    public QuestTaskUI QuestTaskPrefab;
    public Transform QuestTaskContainer;

    private Quest _quest;

    private void Awake()
    {
      button = button ?? GetComponent<ButtonUI>();
      if (button != null)
      {
        button.canvasGroupWatcher = button.canvasGroupWatcher ?? button.GetComponentInParent<CanvasGroupWatcher>();
        if (button.canvasGroupWatcher.AutoFocusButton == null)
          button.AutoFocus = true;
        button.button.onClick.AddListener(Open);
      }
      if (TrackedStatusToggle != null)
        TrackedStatusToggle.onValueChanged.AddListener(SetTrackedStatus);
    }

    public override void OnDestroy()
    {
      base.OnDestroy();
      if (button != null)
        button.button.onClick.RemoveListener(Open);
      if (TrackedStatusToggle != null)
        TrackedStatusToggle.onValueChanged.RemoveListener(SetTrackedStatus);
    }

    protected override void OnPlayerChange(Player _player)
    {
      if (player != null)
      {
        player.Quests.TrackedQuests.OnChange -= OnTrackedChange;
      }
      base.OnPlayerChange(_player);
      if (player == null) return;
      player.Quests.TrackedQuests.OnChange += OnTrackedChange;
      if (_quest == null) return;
      bool tracked = player.Quests.TrackedQuests.Contains(_quest.QuestId);
      if (TrackedStatusToggle != null && TrackedStatusToggle.isOn != tracked)
        TrackedStatusToggle.isOn = player.Quests.TrackedQuests.Contains(_quest.QuestId);
    }

    private void OnTrackedChange(SyncListOperation op, int index, string previous, string next, bool asServer)
    {
      if (asServer) return;
      if (_quest == null) return;
      if (previous != _quest.QuestId && next != _quest.QuestId) return;
      TrackedStatusToggle.isOn = player.Quests.TrackedQuests.Contains(_quest.QuestId);
      if (gameObject.activeInHierarchy)
        StartCoroutine(SelectToggle());
    }

    private void SetQuest(Quest value)
    {
      _quest = value;
      if (_quest == null) return;
      QuestTitle.text = value.Title;
      QuestLevel.text = "LV " + value.Requirement.RequiredLevel;
      if (player != null && TrackedStatusToggle != null)
        TrackedStatusToggle.isOn = player.Quests.TrackedQuests.Contains(_quest.QuestId);
      if (QuestTaskContainer != null)
        foreach (Transform child in QuestTaskContainer)
          Destroy(child.gameObject);
      if (QuestDescription != null)
      {
        QuestDescription.text = value.ShortDescription;
        QuestDescription.gameObject.SetActive(true);
      }
      if (QuestTaskContainer != null)
      {
        QuestTaskContainer.gameObject.SetActive(value.Tasks.Length > 0);

        foreach (var task in value.Tasks)
        {
          var taskUI = Instantiate(QuestTaskPrefab, QuestTaskContainer, false);
          taskUI.Task = task;
        }
      }
    }

    public void OnPointerDown(PointerEventData eventData) => Open();

    public void Open()
    {
      if (_quest == null) return;
      journal.window.moveSibling.ToFront();
      journal.QuestInfoUI.Quest = _quest;
    }


    public void SetTrackedStatus(bool value)
    {
      bool tracked = player.Quests.TrackedQuests.Contains(Quest.QuestId);
      if (value != tracked)
        player.Quests.ToggleTrack(Quest.QuestId, value);
    }

    private IEnumerator SelectToggle()
    {
      yield return new WaitForSeconds(0.1f);
      EventSystem.current.SetSelectedGameObject(TrackedStatusToggle.gameObject);
    }
  }
}