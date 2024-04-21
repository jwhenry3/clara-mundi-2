
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
    public AutoFocus AutoFocus;
    public TextMeshProUGUI QuestTitle;
    public TextMeshProUGUI QuestLevel;
    public TextMeshProUGUI QuestDescription;
    public GameObject TrackedStatusContainer;
    public Toggle TrackedStatusToggle;

    public QuestTaskUI QuestTaskPrefab;
    public Transform QuestTaskContainer;
    public bool IsQuestTackerQuest = false;
    public bool ShowShortDescription;

    public Focusable Focusable;
    private Quest _quest;

    private void Awake()
    {
      Focusable = Focusable ?? GetComponent<Focusable>();
      if (Focusable != null)
        Focusable.OnClick += Open;
      if (IsQuestTackerQuest) return;
      TrackedStatusToggle.onValueChanged.AddListener(SetTrackedStatus);
    }

    public override void OnDestroy()
    {
      base.OnDestroy();
      Focusable.OnClick -= Open;
      if (IsQuestTackerQuest) return;
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
      if (TrackedStatusToggle.isOn != tracked)
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
      TrackedStatusContainer.SetActive(!IsQuestTackerQuest);
      if (!IsQuestTackerQuest && player != null)
        TrackedStatusToggle.isOn = player.Quests.TrackedQuests.Contains(_quest.QuestId);
      QuestTitle.text = value.Title;
      QuestLevel.text = "LV " + value.Requirement.RequiredLevel;
      foreach (Transform child in QuestTaskContainer)
        Destroy(child.gameObject);
      QuestDescription.gameObject.SetActive(IsQuestTackerQuest || ShowShortDescription);
      QuestDescription.text = value.ShortDescription;
      QuestTaskContainer.gameObject.SetActive(IsQuestTackerQuest && value.Tasks.Length > 0);

      if (!IsQuestTackerQuest) return;

      Focusable.IsActivated = false;
      foreach (var task in value.Tasks)
      {
        var taskUI = Instantiate(QuestTaskPrefab, QuestTaskContainer, false);
        taskUI.Task = task;
      }
    }

    public void OnPointerDown(PointerEventData eventData) => Open();

    public void Open()
    {
      if (_quest == null) return;
      QuestJournalUI.Instance.QuestInfoUI.Quest = _quest;
      if (TopLevelCanvas.Instance != null)
      {
        TopLevelCanvas.Instance.Controls["Main"].gameObject.SetActive(true);
        TopLevelCanvas.Instance.Controls["Journal"].gameObject.SetActive(true);
      }
    }

    private void Update()
    {
      if (IsQuestTackerQuest) return;
      Focusable.IsActivated = QuestJournalUI.Instance.QuestInfoUI.Quest == _quest;
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