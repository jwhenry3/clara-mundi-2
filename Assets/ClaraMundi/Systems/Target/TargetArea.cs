using System.Collections.Generic;
using UnityEngine;

namespace ClaraMundi
{
  public class TargetArea : MonoBehaviour
  {

    public Player Player;
    public List<Targetable> InArea = new();
    public List<Targetable> PossibleTargets = new();

    private void OnTriggerEnter(Collider other)
    {
      if (PlayerManager.Instance.LocalPlayer != Player) return;
      Debug.Log("Triggered " + other.gameObject.name);
      var targetable = other.GetComponent<Targetable>();
      if (targetable != null && !InArea.Contains(targetable))
        InArea.Add(targetable);
    }

    private void OnTriggerStay(Collider other)
    {
      if (PlayerManager.Instance.LocalPlayer != Player) return;
      var targetable = other.GetComponent<Targetable>();
      if (targetable == null) return;
      if (!InArea.Contains(targetable))
        InArea.Add(targetable);
      targetable.UpdateDetails();
      bool inList = PossibleTargets.Contains(targetable);
      if (!inList && targetable.OnScreen)
        PossibleTargets.Add(targetable);
      else if (inList && !targetable.OnScreen)
        PossibleTargets.Remove(targetable);
    }

    private void OnTriggerExit(Collider other)
    {
      if (PlayerManager.Instance.LocalPlayer != Player) return;
      var targetable = other.GetComponent<Targetable>();
      if (targetable != null && InArea.Contains(targetable))
      {
        InArea.Remove(targetable);
        targetable.UpdateDetails();
        if (targetable.TargetController != null && targetable.TargetController.SubTargetId.Value == targetable.Entity.entityId.Value)
          targetable.TargetController.SetSubTarget(null);
      }
    }
  }
}