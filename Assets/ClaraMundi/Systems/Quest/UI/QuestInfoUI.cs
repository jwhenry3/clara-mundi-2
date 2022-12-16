using System.Collections.Generic;
using UnityEngine;

namespace ClaraMundi.Quests
{
    public class QuestInfoUI : MonoBehaviour
    {
        public Quest Quest;
        public CompletedQuest Completion;
        public List<QuestTaskProgress> TaskProgress;
    }
}