using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace ClaraMundi.Quests
{
    public class QuestInfoUI : PlayerUI
    {
        [BoxGroup("State")]
        [ShowInInspector]
        public Quest Quest
        {
            get => _quest;
            set => SetQuest(value);
        }
        private Quest _quest;

        public CompletedQuest Completion;
        public List<QuestTaskProgress> TaskProgress;
        [BoxGroup("UI Elements")]
        public QuestRewardUI RewardPrefab;
        [BoxGroup("UI Elements")]
        public Transform RewardsContainer;
        [BoxGroup("UI Elements")]
        public QuestTaskUI TaskPrefab;
        [BoxGroup("UI Elements")]
        public Transform TasksContainer;
        [BoxGroup("UI Elements")]
        public GameObject CurrencyRewardContainer;
        [BoxGroup("UI Elements")]
        public TextMeshProUGUI QuestLevel;
        [BoxGroup("UI Elements")]
        public TextMeshProUGUI QuestName;
        [BoxGroup("UI Elements")]
        public TextMeshProUGUI ShortDescription;
        [BoxGroup("UI Elements")]
        public TextMeshProUGUI CurrencyAmount;
        [BoxGroup("UI Elements")]
        public TextMeshProUGUI Lore;

        public override void Start()
        {
            base.Start();
            gameObject.SetActive(_quest != null);
        }

        protected override void OnPlayerChange(Player _player)
        {
            if (player != null)
            {
                // clean up listeners
            }
            base.OnPlayerChange(_player);
            if (player == null) return;
            // register listeners
            SetQuest(_quest);
        }

        private void SetQuest(Quest quest)
        {
            foreach (Transform child in RewardsContainer)
            {
                var reward = child.GetComponent<QuestRewardUI>();
                if (reward != null)
                    Destroy(child.gameObject);
            }

            foreach (Transform child in TasksContainer)
            {
                var task = child.GetComponent<QuestTaskUI>();
                if (task != null)
                    Destroy(child.gameObject);
            }
            _quest = quest;
            gameObject.SetActive(_quest != null);
            if (player == null) return;
            if (quest == null) return;
            QuestLevel.text = "LV " + quest.Requirement.RequiredLevel;
            QuestName.text = quest.Title;
            ShortDescription.text = quest.ShortDescription;
            Lore.text = quest.Lore;
            // set up task progress
            foreach (var task in quest.Tasks)
                AddTask(task);
            foreach (var reward in quest.Rewards)
                AddReward(reward);

            CurrencyRewardContainer.SetActive(quest.CurrencyReward > 0);
            CurrencyAmount.text = quest.CurrencyReward + "";
        }

        private void AddTask(QuestTask task)
        {
            var instance = Instantiate(TaskPrefab, TasksContainer, false);
            instance.Task = task;
        }

        private void AddReward(LootItem reward)
        {
            var instance = Instantiate(RewardPrefab, RewardsContainer, false);
            instance.LootItem = reward;
        }
    }
}