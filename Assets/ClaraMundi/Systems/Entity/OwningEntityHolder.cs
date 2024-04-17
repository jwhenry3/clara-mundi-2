using System;
namespace ClaraMundi
{
  public class OwningEntityHolder
  {
    public event Action<string, string> EntityChange;
    public Entity entity;

    public void SetEntity(Entity value)
    {

      var lastId = entity != null ? entity.entityId.Value : "";
      entity = value;
      EntityChange?.Invoke(lastId, entity.entityId.Value);
    }
  }
}