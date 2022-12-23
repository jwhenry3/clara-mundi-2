using Unisave.Broadcasting;
using UnityEngine;

public class ChatMessage : BroadcastingMessage
{
    public string messageId;
    public string senderName;
    public string toName;
    public string message;
}