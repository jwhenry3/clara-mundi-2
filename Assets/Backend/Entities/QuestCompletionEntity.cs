using System;
using Unisave.Entities;

public class QuestCompletionEntity : Entity
{
    public EntityReference<CharacterEntity> Character;
    public string QuestId;
    public DateTime LastCompletedTimestamp = DateTime.UtcNow;
}