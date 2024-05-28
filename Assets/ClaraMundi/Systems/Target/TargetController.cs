using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine.InputSystem;
using static UnityEngine.ParticleSystem;
using GameCreator.Runtime.Characters.IK;
using GameCreator.Runtime.Cameras;
namespace ClaraMundi
{
  public class TargetController : PlayerController
  {
    public Transform TargetIndicator;
    private ParticleSystem indicatorParticles;
    private ShapeModule indicatorShape;
    public TargetArea TargetArea;
    public ShotCamera TargetCamera;
    public ShotTypeLockOn lockOnShotType;
    // target must be persisted to server to ensure those observing what the npc/player is targeting can see it
    // also we can use targetless RPCs against the selected target for some simplicity
    public readonly SyncVar<string> TargetId = new(new SyncTypeSettings(WritePermission.ClientUnsynchronized));
    public readonly SyncVar<string> SubTargetId = new(new SyncTypeSettings(WritePermission.ClientUnsynchronized));
    public readonly SyncVar<bool> CameraLockTarget = new(new SyncTypeSettings(WritePermission.ClientUnsynchronized));
    public readonly SyncVar<bool> CameraLockSubTarget = new(new SyncTypeSettings(WritePermission.ClientUnsynchronized));

    public Targetable SubTarget => !string.IsNullOrEmpty(SubTargetId.Value) && EntityManager.Instance != null && EntityManager.Instance.Entities.ContainsKey(SubTargetId.Value) ? EntityManager.Instance.Entities[SubTargetId.Value].GetComponent<Targetable>() : null;

    public Targetable Target => !string.IsNullOrEmpty(TargetId.Value) && EntityManager.Instance != null && EntityManager.Instance.Entities.ContainsKey(TargetId.Value) ? EntityManager.Instance.Entities[TargetId.Value].GetComponent<Targetable>() : null;


    private bool listening;


    private bool nextPressed;
    private bool prevPressed;
    private float inputCooldown;

    private Targetable targetable;

    private RigLookTo lookRig;
    private LookToTransform lookTo;
    private FacingToggle facingToggle;

    public void ServerSetTarget(string targetId)
    {
      if (IsServerInitialized)
        TargetId.Value = targetId;
    }

    [ServerRpc(RunLocally = true)]
    public void SetTarget(string targetId)
    {
      TargetId.Value = targetId;
      if (string.IsNullOrEmpty(targetId))
        CameraLockTarget.Value = false;
      else
      {
        if (!CameraLockTarget.Value)
          CameraLockTarget.Value = CameraLockSubTarget.Value;
        SubTargetId.Value = null;
        CameraLockSubTarget.Value = false;
      }
    }
    [ServerRpc(RunLocally = true)]
    public void SetSubTarget(string targetId)
    {
      SubTargetId.Value = targetId;
      if (string.IsNullOrEmpty(targetId))
        CameraLockSubTarget.Value = false;
    }

    [ServerRpc(RunLocally = true)]
    public void SetLockTarget(bool value)
    {
      if (string.IsNullOrEmpty(TargetId.Value))
        value = false;
      CameraLockTarget.Value = value;
    }
    [ServerRpc(RunLocally = true)]
    public void SetLockSubTarget(bool value)
    {
      if (string.IsNullOrEmpty(SubTargetId.Value))
        value = false;
      CameraLockSubTarget.Value = value;
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
        SetLockSubTarget(!CameraLockSubTarget.Value);
      else if (Target != null)
        SetLockTarget(!CameraLockTarget.Value);
    }

    void Update()
    {
      if (!IsClientStarted) return;
      if (inputCooldown > 0)
      {
        inputCooldown -= Time.deltaTime;
        if (inputCooldown <= 0)
        {
          if (prevPressed)
          {
            SanitizeTargets();
            if (string.IsNullOrEmpty(SubTargetId.Value) && string.IsNullOrEmpty(TargetId.Value))
              TargetArea.PossibleTargets.Sort((a, b) => (int)(a.CenterScore.CompareTo(b.CenterScore) * 100));
            else
              TargetArea.PossibleTargets.Sort((a, b) => (int)(a.Score.CompareTo(b.Score) * 100));
            SetSubTargetAt(GetPreviousIndex());
          }
          else if (nextPressed)
          {
            SanitizeTargets();
            if (string.IsNullOrEmpty(SubTargetId.Value) && string.IsNullOrEmpty(TargetId.Value))
              TargetArea.PossibleTargets.Sort((a, b) => (int)(b.CenterScore.CompareTo(a.CenterScore) * 100));
            else
              TargetArea.PossibleTargets.Sort((a, b) => (int)(a.Score.CompareTo(b.Score) * 100));
            SetSubTargetAt(GetNextIndex());
          }
          prevPressed = false;
          nextPressed = false;
        }
      }
      if (!string.IsNullOrEmpty(SubTargetId.Value) && SubTarget == null)
        SetSubTarget(null);
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

      if (lookRig == null)
        lookRig = player.Body.IK.GetRig<RigLookTo>();
      if (facingToggle == null)
        facingToggle = player.Body.Facing as FacingToggle;
      if (lookRig != null)
      {
        if (SubTarget != null)
        {
          if (lookTo.Target?.transform != SubTarget.transform)
          {
            lookRig.ClearTargets();
            lookRig.SetTarget(new LookToTransform(1, SubTarget.transform, Vector3.zero));
          }
          if (CameraLockSubTarget.Value)
            facingToggle.Target = SubTarget.gameObject;
          else
            facingToggle.Target = null;
        }
        else if (Target != null)
        {
          if (lookTo.Target?.transform != Target.transform)
          {
            lookRig.ClearTargets();
            lookRig.SetTarget(new LookToTransform(1, Target.transform, Vector3.zero));
          }
          if (CameraLockTarget.Value)
            facingToggle.Target = Target.gameObject;
          else
            facingToggle.Target = null;
        }
        else
        {
          lookTo = default;
          lookRig.ClearTargets();
          facingToggle.Target = null;
        }
      }
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
      if (string.IsNullOrEmpty(TargetId.Value) && string.IsNullOrEmpty(SubTargetId.Value))
        OnNextTarget(context);
      else if (!string.IsNullOrEmpty(SubTargetId.Value))
        SetTarget(SubTargetId.Value);
      else
        OpenActionMenu();
    }

    void OpenActionMenu()
    {
      ActionMenuUI.Instance.targetEntity = Target.Entity;
      ActionMenuUI.Instance.window.moveSibling.ToFront();
    }
    void OnCancelTarget(InputAction.CallbackContext context)
    {
      if (!string.IsNullOrEmpty(SubTargetId.Value))
        SetSubTarget(null);
      else if (!string.IsNullOrEmpty(TargetId.Value))
        SetTarget(null);
    }

    void OnNextTarget(InputAction.CallbackContext context)
    {
      inputCooldown = 0.1f;
      nextPressed = true;
    }
    void OnPreviousTarget(InputAction.CallbackContext context)
    {
      inputCooldown = 0.1f;
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
      if (SubTargetId.Value != null)
      {
        if (!EntityManager.Instance.Entities.ContainsKey(SubTargetId.Value))
          SetSubTarget(null);
      }
    }
    int GetIndexOfCurrentTarget()
    {
      if (SubTargetId.Value == null && TargetId.Value == null)
        return -1;
      Entity targetEntity = EntityManager.Instance.Entities[SubTargetId.Value ?? TargetId.Value];
      Targetable targetable = targetEntity.GetComponent<Targetable>();
      return TargetArea.PossibleTargets.IndexOf(targetable);
    }

    void SetSubTargetAt(int index)
    {
      if (index > -1 && TargetArea.PossibleTargets.Count > index)
      {
        TargetArea.PossibleTargets[index].TargetController = this;
        SetSubTarget(TargetArea.PossibleTargets[index].Entity.entityId.Value);
      }
      else
        SetSubTarget(null);
    }




  }
}