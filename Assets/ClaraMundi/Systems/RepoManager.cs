using UnityEngine;

namespace ClaraMundi
{
  public class RepoManager : MonoBehaviour
  {
    public CharacterClassRepo CharacterClassRepo;
    public ItemRepo ItemRepo;
    public QuestRepo QuestRepo;
    public DialogueRepo DialogueRepo;
    public EntityTypeRepo EntityTypeRepo;
    public ActionRepo ActionRepo;
    public static RepoManager Instance;

    private void Awake()
    {
      Instance = this;
    }
  }
}