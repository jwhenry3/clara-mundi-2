using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ClaraMundi.Quests
{
    public class QuestTaskUI : PlayerUI
    {
        public QuestTask Task
        {
            get => _task;
            set => SetTask(value);
        }
        QuestTaskProgress Progress;
        public Sprite InProgressImage;
        public Sprite CompleteImage;
        public Image ProgressImage;
        public Color InProgressColor;
        public Color CompleteColor;
        public TextMeshProUGUI Description;
        public TextMeshProUGUI ProgressText;
        private QuestTask _task;

        protected override void OnPlayerChange(Player _player)
        {
            if (player != null)
            {
                player.Quests.TaskProgress.OnChange -= OnProgressChange;
            }
            base.OnPlayerChange(_player);
            if (player == null) return;
            player.Quests.TaskProgress.OnChange += OnProgressChange;
            if (_task == null) return;
            SetTask(_task);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (player != null)
            {
                player.Quests.TaskProgress.OnChange -= OnProgressChange;
            }
        }

        private void OnProgressChange(SyncDictionaryOperation op, string key, QuestTaskProgress progress, bool asServer)
        {
            if (asServer) return;
            if (_task == null) return;
            if (key != _task.QuestTaskId) return;
            Progress = progress ?? new();
            // update the UI
            SetTask(_task, false);
        }
        private void SetTask(QuestTask task, bool updateProgress = true)
        {
            _task = task;
            gameObject.SetActive(_task != null);
            if (player == null) return;
            if (_task == null) return;
            if (updateProgress)
            {
                if (!player.Quests.TaskProgress.ContainsKey(_task.QuestTaskId))
                    Progress = new();
                else
                    Progress = player.Quests.TaskProgress[_task.QuestTaskId];
            }

            Description.text = _task.ShortDescription;
            switch (_task.Type)
            {
                case QuestTaskType.Dispatch:
                    ProgressText.text = $"({Progress.DispatchCount}/{_task.DispatchQuantity})";
                    break;
                case QuestTaskType.Gather:
                    ProgressText.text = $"({Progress.ItemsHeld}/{_task.ItemQuantity})";
                    break;
                default:
                    ProgressText.text = "";
                    break;
            }

            if (Progress.IsComplete)
            {
                Description.color = CompleteColor;
                ProgressImage.sprite = CompleteImage;
            }
            else
            {
                Description.color = InProgressColor;
                ProgressImage.sprite = InProgressImage;
            }
        }
    }
}