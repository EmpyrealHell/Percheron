using System;
using System.Collections.Generic;
using System.Text;

namespace Percheron.Interfaces.Chat
{
    public struct ChatMessage
    {
        public string Sender { get; set; }
        public string Channel { get; set; }
        public IDictionary<string, string> Tags { get; set; }
        public string Message { get; set; }

        public ChatMessage(string sender, string channel, string message, IDictionary<string, string> tags)
        {
            this.Sender = sender;
            this.Channel = channel;
            this.Message = message;
            this.Tags = tags;
        }
    }
}
