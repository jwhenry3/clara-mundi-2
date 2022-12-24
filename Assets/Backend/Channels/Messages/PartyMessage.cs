using Unisave.Broadcasting;

public enum PartyMessageType
{
    PlayerJoined,
    PlayerLeft,
    Private_PlayerRequestedInvite,
    Private_PlayerInvited,
    Private_PlayerDeclinedInvite,
    Private_PlayerCancelledRequest,
    PartyDisbanded,
    Private_PartyCreated,
    LeaderChanged,
    Private_JoinedParty,
    Private_InvitedToParty,
    Private_LeftParty,
    Private_RequestedInvite,
    Private_RequestDenied,
    Private_AlreadyInParty,
    PartyFull,
}
public class PartyMessage : BroadcastingMessage
{
    public PartyMessageType type;
    public string characterName;
    public string partyId;
}