using Unisave.Broadcasting;

public enum PartyMessageType
{
    PlayerJoined,
    PlayerLeft,
    PlayerRequestedInvite,
    PlayerInvited,
    PlayerDeclinedInvite,
    PlayerCancelledRequest,
    PartyDisbanded,
    PartyCreated,
    LeaderChanged,
    JoinedParty,
    InvitedToParty,
    LeftParty,
    RequestedInvite,
    PartyFull,
}
public class PartyMessage : BroadcastingMessage
{
    public PartyMessageType type;
    public string characterId;
    public string characterName;
}