using System;
namespace ClaraMundi
{
    public class OwningEntityHolder
    {
        public event Action EntityChange;
        public Entity entity;

        public void SetEntity(Entity value)
        {
            entity = value;
            EntityChange?.Invoke();
        }
    }
}