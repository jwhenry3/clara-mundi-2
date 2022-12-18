
using System;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ClaraMundi.Quests
{
    public class QuestListItemUI : PlayerUI, IPointerClickHandler
    {
        public Quest Quest
        {
            get => _quest;
            set => SetQuest(value);
        }

        public TextMeshProUGUI QuestTitle;
        public TextMeshProUGUI QuestLevel;
        public TextMeshProUGUI QuestDescription;
        public Image ActiveImage;
        public GameObject TrackedStatusContainer;
        public Toggle TrackedStatusToggle;
        
        public QuestTaskUI QuestTaskPrefab;
        public Transform QuestTaskContainer;
        public bool IsQuestTackerQuest = false;
        public bool ShowShortDescription;
        private Quest _quest;

        private void Awake()
        {
            if (IsQuestTackerQuest) return;
            TrackedStatusToggle.onValueChanged.AddListener(SetTrackedStatus);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
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
            TrackedStatusToggle.isOn = player.Quests.TrackedQuests.Contains(_quest.QuestId);
        }

        private void OnTrackedChange(SyncListOperation op, int index, string previous, string next, bool asServer)
        {
            if (asServer) return;
            if (_quest == null) return;
            if (previous != _quest.QuestId && next != _quest.QuestId) return;
            TrackedStatusToggle.isOn = player.Quests.TrackedQuests.Contains(_quest.QuestId);
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
            ActiveImage.gameObject.SetActive(false);
            foreach (var task in value.Tasks)
            {
                var taskUI = Instantiate(QuestTaskPrefab, QuestTaskContainer, false);
                taskUI.Task = task;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_quest == null) return;
            QuestJournalUI.Instance.QuestInfoUI.Quest = _quest;
            if (!GameWindowHandler.Instance.Tabs.IsTabActive("Journal"))
                GameWindowHandler.Instance.Tabs.ChangeTab("Journal");
        }

        private void Update()
        {
            if (IsQuestTackerQuest) return;
            if (!ActiveImage.gameObject.activeInHierarchy && QuestJournalUI.Instance.QuestInfoUI.Quest == _quest)
                ActiveImage.gameObject.SetActive(true);
            if (ActiveImage.gameObject.activeInHierarchy && QuestJournalUI.Instance.QuestInfoUI.Quest != _quest)
                ActiveImage.gameObject.SetActive(false);
        }

        public void SetTrackedStatus(bool value)
        {
            if (value)
                player.Quests.TrackQuest(Quest.QuestId);
            else
                player.Quests.UntrackQuest(Quest.QuestId);
        }
    }
}