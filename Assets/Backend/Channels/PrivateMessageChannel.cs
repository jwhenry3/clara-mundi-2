using Unisave.Broadcasting;

public class PrivateMessageChannel : BroadcastingChannel
{
    public SpecificChannel WithParameters(string playerName)
    {
        return SpecificChannel.From<PrivateMessageChannel>(playerName);
    }
}