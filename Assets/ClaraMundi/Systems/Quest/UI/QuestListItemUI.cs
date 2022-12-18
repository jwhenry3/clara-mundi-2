
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
        
        public QuestTaskUI QuestTaskPrefab;
        public Transform QuestTaskContainer;
        public bool IsQuestTackerQuest = false;
        public bool ShowShortDescription;
        private Quest _quest;

        private void SetQuest(Quest value)
        {
            _quest = value;
            QuestTitle.text = value.Title;
            QuestLevel.text = "LV " + value.Requirement.RequiredLevel;
            foreach (Transform child in QuestTaskContainer)
                Destroy(child.gameObject);
            QuestDescription.gameObject.SetActive(IsQuestTackerQuest || ShowShortDescription);
            QuestDescription.text = value.ShortDescription;
            QuestTaskContainer.gameObject.SetActive(IsQuestTackerQuest && value.Tasks.Length > 0);
            if (!IsQuestTackerQuest) return;
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
        }

        private void Update()
        {
            if (!ActiveImage.gameObject.activeInHierarchy && QuestJournalUI.Instance.QuestInfoUI.Quest == _quest)
                ActiveImage.gameObject.SetActive(true);
            if (ActiveImage.gameObject.activeInHierarchy && QuestJournalUI.Instance.QuestInfoUI.Quest != _quest)
                ActiveImage.gameObject.SetActive(false);
        }
    }
}