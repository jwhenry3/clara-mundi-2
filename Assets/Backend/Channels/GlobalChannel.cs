using Unisave.Broadcasting;

public class GlobalChannel : BroadcastingChannel
{
    public SpecificChannel WithParameters(string roomName)
    {
        return SpecificChannel.From<GlobalChannel>(roomName);
    }
}