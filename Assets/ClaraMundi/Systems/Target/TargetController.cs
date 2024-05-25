using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine.InputSystem;
using static UnityEngine.ParticleSystem;
namespace ClaraMundi
{
  public class TargetController : PlayerController
  {
    public Transform TargetIndicator;
    private ParticleSystem indicatorParticles;
    private ShapeModule indicatorShape;
    public TargetArea TargetArea;
    // target must be persisted to server to ensure those observing what the npc/player is targeting can see it
    // also we can use targetless RPCs against the selected target for some simplicity
    public readonly SyncVar<string> TargetId = new(new SyncTypeSettings(WritePermission.ClientUnsynchronized));

    // SubTarget is only needed for actions not performed on the target
    public string SubTargetId;

    public Targetable SubTarget => !string.IsNullOrEmpty(SubTargetId) && EntityManager.Instance != null && EntityManager.Instance.Entities.ContainsKey(SubTargetId) ? EntityManager.Instance.Entities[SubTargetId].GetComponent<Targetable>() : null;

    public Targetable Target => !string.IsNullOrEmpty(TargetId.Value) && EntityManager.Instance != null && EntityManager.Instance.Entities.ContainsKey(TargetId.Value) ? EntityManager.Instance.Entities[TargetId.Value].GetComponent<Targetable>() : null;

    public bool cameraLockTarget;
    public bool cameraLockSubTarget;

    private bool listening;

    private bool nextPressed;
    private bool prevPressed;
    private float inputCooldown;

    private Targetable targetable;

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

    void OnEnable()
    {
      if (player != null)
        targetable = targetable ?? player.GetComponent<Targetable>();
      indicatorParticles = TargetIndicator.GetComponent<ParticleSystem>();
      indicatorShape = indicatorParticles.shape;
    }

    public override void OnStartClient()
    {
      base.OnStartClient();
      if (TargetArea != null)
        TargetArea.enabled = player == PlayerManager.Instance?.LocalPlayer;
      if (PlayerManager.Instance?.LocalPlayer != player)
        return;
      listening = true;
      InputManager.Instance.World.FindAction("Cancel").performed += OnCancelTarget;
      InputManager.Instance.World.FindAction("Confirm").performed += OnConfirm;
      InputManager.Instance.World.FindAction("TargetLock").performed += OnLockTarget;
      InputManager.Instance.World.FindAction("TargetNext").performed += OnNextTarget;
      InputManager.Instance.World.FindAction("TargetPrevious").performed += OnPreviousTarget;
    }

    void OnLockTarget(InputAction.CallbackContext context)
    {
      if (SubTarget != null)
      {
        cameraLockSubTarget = !cameraLockSubTarget;
      }
      else if (Target != null)
      {
        cameraLockTarget = !cameraLockTarget;
      }

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
            TargetArea.PossibleTargets.Sort();
            SetSubTargetAt(GetPreviousIndex());
          }
          else if (nextPressed)
          {
            SanitizeTargets();
            TargetArea.PossibleTargets.Sort();
            SetSubTargetAt(GetNextIndex());
          }
          prevPressed = false;
          nextPressed = false;
        }
      }
      if (!string.IsNullOrEmpty(SubTargetId) && SubTarget == null)
        SubTargetId = null;
      if (!string.IsNullOrEmpty(TargetId.Value) && Target == null)
        SetTarget(null);
      if (TargetIndicator.parent != null)
        TargetIndicator.SetParent(null);
      if (Target != null)
      {
        TargetIndicator.position = Target.TargetIndicatorPosition.position;
        indicatorShape.radius = Target.IndicatorRadius;
        TargetIndicator.gameObject.SetActive(true);
      }
      else
      {
        TargetIndicator.gameObject.SetActive(false);
        indicatorShape.radius = targetable.IndicatorRadius;
        TargetIndicator.position = targetable.TargetIndicatorPosition.position;
      }
      if (Target == null)
        cameraLockTarget = false;
      if (SubTarget == null)
        cameraLockSubTarget = false;
    }

    public void OnDestroy()
    {
      if (TargetIndicator != null)
        Destroy(TargetIndicator.gameObject);
      if (!listening) return;
      InputManager.Instance.World.FindAction("Cancel").performed -= OnCancelTarget;
      InputManager.Instance.World.FindAction("Confirm").performed -= OnConfirm;
      InputManager.Instance.World.FindAction("TargetLock").performed -= OnLockTarget;
      InputManager.Instance.World.FindAction("TargetNext").performed -= OnNextTarget;
      InputManager.Instance.World.FindAction("TargetPrevious").performed -= OnPreviousTarget;
    }
    void OnConfirm(InputAction.CallbackContext context)
    {
      if (string.IsNullOrEmpty(TargetId.Value) && string.IsNullOrEmpty(SubTargetId))
        OnNextTarget(context);
      else if (!string.IsNullOrEmpty(SubTargetId))
      {
        SetTarget(SubTargetId);
        if (cameraLockSubTarget)
          cameraLockTarget = true;
        SubTargetId = null;
      }
    }
    void OnCancelTarget(InputAction.CallbackContext context)
    {
      if (!string.IsNullOrEmpty(TargetId.Value))
        SetTarget(null);
      else
        SubTargetId = null;
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
      if (index > -1 && TargetArea.PossibleTargets.Count > index)
      {
        TargetArea.PossibleTargets[index].TargetController = this;
        SubTargetId = TargetArea.PossibleTargets[index].Entity.entityId.Value;
      }
      else
        SubTargetId = null;
    }




  }
}