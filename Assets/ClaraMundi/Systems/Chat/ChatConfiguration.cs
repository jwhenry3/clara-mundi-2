using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClaraMundi
{
    [Serializable]
    public class ChatColor
    {
        public ChatMessageType MessageType;
        public Color Color;
    }
    [Serializable]
    public class ChannelColor
    {
        public string Channel;
        public Color Color;
    }
    [CreateAssetMenu(fileName = "ChatConfiguration", menuName = "Clara Mundi/Chat/ChatConfiguration")]
    [Serializable]
    public class ChatConfiguration : ScriptableObject
    {
        public ChatColor[] ChatColors;
        public ChannelColor[] ChannelColors;

        public readonly Dictionary<string, Color> ChannelTypeDict = new();
        public readonly Dictionary<ChatMessageType, Color> MessageTypeDict = new();

        public void OnEnable()
        {
            foreach (var c in ChannelColors)
                ChannelTypeDict[c.Channel] = c.Color;
            foreach (var c in ChatColors)
                MessageTypeDict[c.MessageType] = c.Color;
        }
        public string GetColor(ChatMessageType type, string channel)
        {
            switch (type)
            {
                case ChatMessageType.Combat:
                    return  "#" + ColorUtility.ToHtmlStringRGB(MessageTypeDict[ChatMessageType.Combat]);
                case ChatMessageType.Emote:
                    return "#" + ColorUtility.ToHtmlStringRGB(MessageTypeDict[ChatMessageType.Emote]);
                case ChatMessageType.System:
                    return "#" + ColorUtility.ToHtmlStringRGB(MessageTypeDict[ChatMessageType.System]);
                default:
                {
                    if (ChannelTypeDict.ContainsKey(channel))
                        return "#" + ColorUtility.ToHtmlStringRGB(ChannelTypeDict[channel]);
                    else
                        return "#ffffff";
                }
            }
        }
    }
}