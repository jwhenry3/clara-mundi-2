using System.Collections.Generic;
using UnityEngine;

namespace ClaraMundi
{
  public class NPCManager : MonoBehaviour
  {
    public static NPCManager Instance;

    public Dictionary<string, NPC> NPCs = new();
    public Dictionary<string, NPC> NPCsByName = new();

    private void Awake()
    {
      Instance = this;
    }

    public void SetNPC(NPC npc)
    {
      NPCs[npc.Entity.entityId.Value] = npc;
      NPCsByName[npc.Entity.entityName.Value.ToLower().Replace(" ", "")] = npc;
    }

    public NPC GetNPC(string idOrName)
    {
      if (NPCs.ContainsKey(idOrName))
        return NPCs[idOrName];
      if (NPCsByName.ContainsKey(idOrName.ToLower().Replace(" ", "")))
        return NPCsByName[idOrName.ToLower().Replace(" ", "")];
      return null;
    }
  }
}