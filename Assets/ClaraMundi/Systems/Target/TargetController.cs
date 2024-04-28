using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
namespace ClaraMundi
{
  public class TargetController : NetworkBehaviour
  {
    // target must be persisted to server to ensure those observing what the npc/player is targeting can see it
    // also we can use targetless RPCs against the selected target for some simplicity
    public readonly SyncVar<string> TargetId = new();

    // SubTarget is only needed for actions not performed on the target
    public string SubTargetId;

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


  }
}