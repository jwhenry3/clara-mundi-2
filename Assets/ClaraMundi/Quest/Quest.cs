using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace ClaraMundi
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Clara Mundi/Quest/Quest")]
    [Serializable]
    public class Quest : ScriptableObject
    {
        public string QuestId = Guid.NewGuid().ToString();
        [BoxGroup("Identity")]
        public string Title = "";
        [BoxGroup("Identity")]
        public string ShortDescription = "";
        [FormerlySerializedAs("QuestStarter")] [BoxGroup("Identity")]
        public Dialogue Starter;
        [BoxGroup("Identity")]
        [Multiline(10)]
        public string Lore = "";
        [BoxGroup("Requirements")]
        public QuestRequirement Requirement;
        [TabGroup("Tasks")]
        public QuestTask[] Tasks;
        [TabGroup("Rewards")]
        public LootItem[] Rewards;
        [TabGroup("Rewards")]
        public int CurrencyReward = 0;
        // More reward types can be added as systems are implemented
        // IE: faction standing, guild experience, etc

        // Immediate quests that follow this quest
        public List<Quest> NextQuests => RepoManager.Instance.QuestRepo.GetOrCreateNextQuestList(QuestId);


        public void OnEnable()
        {
            if (Starter != null)
                Starter.AssociatedQuestId = QuestId;
        }
    }
}