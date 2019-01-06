using System;
using System.Collections.Generic;
using System.Text;

namespace Percheron.Core.Chat
{
    public class ChatMessage
    {
        public string Sender { get; set; }
        public string Channel { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public string Message { get; set; }
    }
}
