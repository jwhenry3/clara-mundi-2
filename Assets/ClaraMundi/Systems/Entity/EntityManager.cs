using System.Collections.Generic;
using UnityEngine;

namespace ClaraMundi
{
  public class EntityManager : MonoBehaviour
  {
    public Dictionary<string, Entity> Entities = new();
    public Dictionary<string, Entity> EntitiesByName = new();
    public static EntityManager Instance;

    private void Awake()
    {
      Instance = this;
    }
    public void ChangeName(string prev, string next, Entity entity)
    {
      if (prev != null && EntitiesByName.ContainsKey(prev) && EntitiesByName[prev] == entity)
        EntitiesByName.Remove(prev);
      if (next != null)
        EntitiesByName[next.ToLower()] = entity;
    }
    public void ChangeId(string prev, string next, Entity entity)
    {
      if (prev != null && Entities.ContainsKey(prev) && Entities[prev] == entity)
        Entities.Remove(prev);
      if (next != null)
        Entities[next] = entity;
    }
    public void RemoveEntity(Entity entity)
    {
      if (Entities.ContainsKey(entity.entityId.Value))
        Entities.Remove(entity.entityId.Value);
      if (EntitiesByName.ContainsKey(entity.entityName.Value.ToLower()))
        EntitiesByName.Remove(entity.entityName.Value.ToLower());
    }
    public void SetEntity(Entity entity)
    {
      Entities[entity.entityId.Value] = entity;
      EntitiesByName[entity.entityName.Value.ToLower()] = entity;
    }

    public Entity GetEntity(string idOrName)
    {
      if (string.IsNullOrEmpty(idOrName)) return null;
      if (Entities.ContainsKey(idOrName))
        return Entities[idOrName];
      if (EntitiesByName.ContainsKey(idOrName.ToLower()))
        return EntitiesByName[idOrName.ToLower()];
      return null;
    }

    public T GetEntityComponent<T>(string idOrName) where T : MonoBehaviour
    {
      var entity = GetEntity(idOrName);
      if (entity == null) return null;
      return entity.GetComponent<T>();
    }
  }
}