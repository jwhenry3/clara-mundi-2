using UnityEngine;

namespace ClaraMundi
{
    public class RepoManager : MonoBehaviour
    {
        public RegionRepo RegionRepo;
        public ItemRepo ItemRepo;
        public QuestRepo QuestRepo;
        public DialogueRepo DialogueRepo;
        public EntityTypeRepo EntityTypeRepo;
        public static RepoManager Instance;

        private void Awake()
        {
            Instance = this;
        }
    }
}