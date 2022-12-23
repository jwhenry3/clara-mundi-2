using Unisave.Entities;

public class PartyMemberEntity : Entity
{
    public string PartyId;
    public EntityReference<CharacterEntity> Character;
    public EntityReference<CharacterEntity> Leader;
    public bool IsInvited;
    public bool IsRequested;
    public bool HasJoined;
}