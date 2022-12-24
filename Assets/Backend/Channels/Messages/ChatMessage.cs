﻿using Unisave.Broadcasting;

namespace Backend
{
    public class ChatMessage : BroadcastingMessage
    {
        public string messageId;
        public string senderArea;
        public string senderName;
        public string toName;
        public string message;
    }
}