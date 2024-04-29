using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine.InputSystem;
namespace ClaraMundi
{
  public class TargetController : PlayerController
  {
    public InputActionAsset InputActionAsset;
    public TargetArea TargetArea;
    // target must be persisted to server to ensure those observing what the npc/player is targeting can see it
    // also we can use targetless RPCs against the selected target for some simplicity
    public readonly SyncVar<string> TargetId = new();

    // SubTarget is only needed for actions not performed on the target
    public string SubTargetId;

    private bool listening;

    private bool nextPressed;
    private bool prevPressed;
    private float inputCooldown;

    public void ServerSetTarget(string targetId)
    {
      if (IsServerInitialized)
        TargetId.Value = targetId;
    }

    [ServerRpc(RunLocally = true)]
    public void SetTarget(string targetId)
    {
      TargetId.Value = targetId;
    }

    public override void OnStartClient()
    {
      base.OnStartClient();
      if (TargetArea != null)
        TargetArea.enabled = player == PlayerManager.Instance?.LocalPlayer;
      if (PlayerManager.Instance?.LocalPlayer != player)
        return;
      listening = true;
      InputActionAsset.FindAction("Player/TargetNext").performed += OnNextTarget;
      InputActionAsset.FindAction("Player/TargetPrevious").performed += OnPreviousTarget;
    }

    void Update()
    {
      if (inputCooldown > 0)
      {
        inputCooldown -= Time.deltaTime;
        if (inputCooldown <= 0)
        {
          if (prevPressed)
          {
            SanitizeTargets();
            SetSubTargetAt(GetNextIndex());
          }
          else if (nextPressed)
          {
            SanitizeTargets();
            SetSubTargetAt(GetPreviousIndex());
          }
          prevPressed = false;
          nextPressed = false;
        }
      }
    }

    public void OnDestroy()
    {
      if (!listening) return;
      InputActionAsset.FindAction("Player/TargetNext").performed -= OnNextTarget;
      InputActionAsset.FindAction("Player/TargetPrevious").performed -= OnPreviousTarget;
    }

    void OnNextTarget(InputAction.CallbackContext context)
    {
      inputCooldown = 0.3f;
      nextPressed = true;
    }
    void OnPreviousTarget(InputAction.CallbackContext context)
    {
      inputCooldown = 0.3f;
      prevPressed = true;
    }
    int GetPreviousIndex()
    {
      int index = GetIndexOfCurrentTarget();
      if (index > 0)
        return index - 1;
      return TargetArea.PossibleTargets.Count - 1;
    }
    int GetNextIndex()
    {
      int index = GetIndexOfCurrentTarget();
      if (index > -1 && index < TargetArea.PossibleTargets.Count - 1)
        return index + 1;
      return 0;
    }
    void SanitizeTargets()
    {
      if (TargetId.Value != null)
      {
        if (!EntityManager.Instance.Entities.ContainsKey(TargetId.Value))
          SetTarget(null);
      }
      if (SubTargetId != null)
      {
        if (!EntityManager.Instance.Entities.ContainsKey(SubTargetId))
          SubTargetId = null;
      }
    }
    int GetIndexOfCurrentTarget()
    {
      if (SubTargetId == null && TargetId.Value == null)
        return -1;
      Entity targetEntity = EntityManager.Instance.Entities[SubTargetId ?? TargetId.Value];
      Targetable targetable = targetEntity.GetComponent<Targetable>();
      return TargetArea.PossibleTargets.IndexOf(targetable);
    }

    void SetSubTargetAt(int index)
    {
      if (index > 0 && TargetArea.PossibleTargets.Count > index)
        SubTargetId = TargetArea.PossibleTargets[index].Entity.entityId.Value;
      else
        SubTargetId = null;
    }

  }
}