using System;
using System.Collections.Generic;
using System.Text;

namespace Percheron.Interfaces.Chat
{
    public class ChatZone
    {
        public ChatZoneType ZoneType { get; set; }
        public string Name { get; set; }
    }

    public enum ChatZoneType
    {
        Channel,
        Room,
        Whisper
    }
}
