using Unisave.Broadcasting;

public class PartyChannel : BroadcastingChannel
{
    public SpecificChannel WithParameters(string leaderName)
    {
        return SpecificChannel.From<PartyChannel>(leaderName);
    }
}