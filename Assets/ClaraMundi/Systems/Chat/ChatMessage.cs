using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ClaraMundi
{
    public enum ChatMessageType
    {
        System,
        Chat,
        Combat,
        Emote
    }

    public enum ChannelScope
    {
        Local,
        Private,
        Scene,
        Global,
    }

    [Serializable]
    public class ChatMessage
    {
        public string MessageId = Guid.NewGuid().ToString();
        public ChatMessageType Type = ChatMessageType.System;
        public string Channel = "Say";
        public string SenderEntityId;
        public Vector3 SenderPosition;
        // used in private message to show the recipient's name in the chat
        public string ToEntityId;
        public string Message = "";
    }
}