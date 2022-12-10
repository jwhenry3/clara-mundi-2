using System;
using System.Collections.Generic;
using UnityEngine;

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
    public enum ChannelType
    {
        Whisper,
        Trade,
        LFG,
        Shout,
        Say,
        Yell,
        Guild,
        Party,
        System,

    }

    [Serializable]
    public class ChatMessage
    {
        public ChatMessageType Type = ChatMessageType.System;
        public ChannelType ChannelType = ChannelType.System;
        public string SenderEntityId;
        public Vector3 SenderPosition;
        // used in private message to show the recipient's name in the chat
        public string ToEntityId;
        public string Message = "";
    }
}