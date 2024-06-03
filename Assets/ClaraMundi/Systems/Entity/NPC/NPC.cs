using UnityEngine;

namespace ClaraMundi
{
  public class NPC : MonoBehaviour
  {
    public Entity Entity;

    void OnEnable()
    {
      Entity ??= GetComponent<Entity>();
      Entity.OnStarted += OnStarted;
    }
    void OnStarted()
    {
      NPCManager.Instance.SetNPC(this);
    }
  }
}