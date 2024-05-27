using System;
using UnityEngine;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Characters;

namespace ClaraMundi
{
  [Title("Facing Toggle")]
  [Image(typeof(IconCubeSolid), ColorTheme.Type.Blue)]

  [Category("Targets/Facing Toggle")]
  [Description("Rotates the Character towards a specific game object target")]

  [Serializable]
  public class FacingToggle : TUnitFacing
  {

    private enum DirectionFrom
    {
      MotionDirection,
      DriverDirection
    }
    // EXPOSED MEMBERS: -----------------------------------------------------------------------
    public GameObject Target;
    [SerializeField] private DirectionFrom m_DirectionFrom = DirectionFrom.MotionDirection;
    [SerializeField] private Axonometry m_Axonometry = new Axonometry();

    // PROPERTIES: ----------------------------------------------------------------------------

    public override Axonometry Axonometry
    {
      get => null;
      set => _ = value;
    }
    // METHODS: -------------------------------------------------------------------------------

    protected override Vector3 GetDefaultDirection()
    {
      if (!Target)
      {
        Vector3 driverDirection = Vector3.Scale(
            m_DirectionFrom switch
            {
              DirectionFrom.MotionDirection => Character.Motion.MoveDirection,
              DirectionFrom.DriverDirection => Character.Driver.WorldMoveDirection,
              _ => throw new ArgumentOutOfRangeException()
            },
            Vector3Plane.NormalUp
        );

        Vector3 direction = DecideDirection(driverDirection);
        return m_Axonometry?.ProcessRotation(this, direction) ?? direction;
      }
      else
      {

        Vector3 driverDirection = Vector3.Scale(
            Target != null
                ? Target.transform.position - Transform.position
                : Character.Driver.WorldMoveDirection,
            Vector3Plane.NormalUp
        );

        return DecideDirection(driverDirection);
      }
    }

    // STRING: --------------------------------------------------------------------------------

    public override string ToString() => "Facing Toggle";
  }
}