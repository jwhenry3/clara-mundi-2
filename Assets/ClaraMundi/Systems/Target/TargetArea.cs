using System.Collections.Generic;
using UnityEngine;

namespace ClaraMundi
{
  public class TargetArea : MonoBehaviour
  {

    public Player Player;
    public List<Targetable> InArea = new();
    public List<Targetable> PossibleTargets = new();

    public float updateInterval = 0.5f;
    private float currentTick = 0;

    private void OnEnable()
    {
      if (PlayerManager.Instance?.LocalPlayer != Player)
        enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
      if (PlayerManager.Instance?.LocalPlayer == null) return;
      var targetable = other.GetComponent<Targetable>();
      if (targetable != null && !InArea.Contains(targetable))
        InArea.Add(targetable);
    }
    private void OnTriggerStay(Collider other)
    {
      if (PlayerManager.Instance?.LocalPlayer == null) return;
      var targetable = other.GetComponent<Targetable>();
      if (targetable != null && InArea.Contains(targetable))
      {
        bool inList = PossibleTargets.Contains(targetable);
        if (!inList && targetable.OnScreen)
          PossibleTargets.Add(targetable);
        else if (inList && !targetable.OnScreen)
          PossibleTargets.Remove(targetable);
      }
    }
    private void OnTriggerExit(Collider other)
    {
      if (PlayerManager.Instance?.LocalPlayer == null) return;
      var targetable = other.GetComponent<Targetable>();
      if (targetable != null && InArea.Contains(targetable))
        InArea.Remove(targetable);
    }

    private void LateUpdate()
    {
      if (PlayerManager.Instance?.LocalPlayer == null) return;
      currentTick += Time.deltaTime;
      if (currentTick > updateInterval)
      {
        currentTick = 0;
        PossibleTargets.Sort();
      }
    }
  }
}