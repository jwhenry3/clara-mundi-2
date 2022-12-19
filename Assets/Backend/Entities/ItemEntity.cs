using Unisave.Entities;

public class ItemEntity : Entity
{
    public EntityReference<CharacterEntity> Character;
    public string StorageId;
    public string ItemId;
    public int Quantity;
    public bool IsEquipped;
}
