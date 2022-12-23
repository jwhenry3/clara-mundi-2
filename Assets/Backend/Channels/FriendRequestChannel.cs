using Unisave.Broadcasting;

public class FriendRequestChannel : BroadcastingChannel
{
    public SpecificChannel WithParameters(string playerName)
    {
        return SpecificChannel.From<FriendRequestChannel>(playerName);
    }
}