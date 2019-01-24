using Percheron.Interfaces.Chat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Percheron.Core.Chat
{
    /// <summary>
    /// A client for connecting to Twitch chat through IRC.
    /// </summary>
    public class TwitchChatClient : ITwitchClient
    {
        private IDictionary<string, Action<ITwitchClient, ChatMessage>> commandMap;
        private string username;

        public IIrcClient Client { get; private set; }

        public event TwitchEvent OnChatEvent;

        public event TwitchEvent OnPrivMsg;
        public event TwitchEvent OnJoin;
        public event TwitchEvent OnPart;
        public event TwitchEvent OnMode;
        public event TwitchEvent OnNames;
        public event TwitchEvent OnNamesComplete;
        public event TwitchEvent OnClearChat;
        public event TwitchEvent OnClearMsg;
        public event TwitchEvent OnHostTarget;
        public event TwitchEvent OnNotice;
        public event TwitchEvent OnReconnect;
        public event TwitchEvent OnRoomState;
        public event TwitchEvent OnUserNotice;
        public event TwitchEvent OnUserState;
        public event TwitchEvent OnGlobalUserState;

        public event Connected OnConnect;

        /// <summary>
        /// Creates a client for connecting to twitch chat. This client will log all messages between the client and the server to the console, to aid debugging.
        /// </summary>
        public TwitchChatClient() : this(Console.OpenStandardOutput())
        {
        }

        /// <summary>
        /// Creates a client for connecting to twitch chat. This client will log all messages between the client and the server to the specified stream, if it isn't null.
        /// </summary>
        public TwitchChatClient(Stream debug = null) : this(new IrcClient(debug))
        {
        }

        public TwitchChatClient(IIrcClient client)
        {
            this.commandMap = new Dictionary<string, Action<ITwitchClient, ChatMessage>>();
            this.commandMap.Add("PRIVMSG", new Action<ITwitchClient, ChatMessage>((sender, message) => { this.OnPrivMsg?.Invoke(sender, message); }));
            this.commandMap.Add("JOIN", new Action<ITwitchClient, ChatMessage>((sender, message) => { this.OnJoin?.Invoke(sender, message); }));
            this.commandMap.Add("PART", new Action<ITwitchClient, ChatMessage>((sender, message) => { this.OnPart?.Invoke(sender, message); }));
            this.commandMap.Add("MODE", new Action<ITwitchClient, ChatMessage>((sender, message) => { this.OnMode?.Invoke(sender, message); }));
            this.commandMap.Add("353", new Action<ITwitchClient, ChatMessage>((sender, message) => { this.OnNames?.Invoke(sender, message); }));
            this.commandMap.Add("366", new Action<ITwitchClient, ChatMessage>((sender, message) => { this.OnNamesComplete?.Invoke(sender, message); }));
            this.commandMap.Add("CLEARCHAT", new Action<ITwitchClient, ChatMessage>((sender, message) => { this.OnClearChat?.Invoke(sender, message); }));
            this.commandMap.Add("CLEARMSG", new Action<ITwitchClient, ChatMessage>((sender, message) => { this.OnClearMsg?.Invoke(sender, message); }));
            this.commandMap.Add("HOSTTARGET", new Action<ITwitchClient, ChatMessage>((sender, message) => { this.OnHostTarget?.Invoke(sender, message); }));
            this.commandMap.Add("NOTICE", new Action<ITwitchClient, ChatMessage>((sender, message) => { this.OnNotice?.Invoke(sender, message); }));
            this.commandMap.Add("RECONNECT", new Action<ITwitchClient, ChatMessage>((sender, message) => { this.OnReconnect?.Invoke(sender, message); }));
            this.commandMap.Add("ROOMSTATE", new Action<ITwitchClient, ChatMessage>((sender, message) => { this.OnRoomState?.Invoke(sender, message); }));
            this.commandMap.Add("USERNOTICE", new Action<ITwitchClient, ChatMessage>((sender, message) => { this.OnUserNotice?.Invoke(sender, message); }));
            this.commandMap.Add("USERSTATE", new Action<ITwitchClient, ChatMessage>((sender, message) => { this.OnUserState?.Invoke(sender, message); }));
            this.commandMap.Add("GLOBALUSERSTATE", new Action<ITwitchClient, ChatMessage>((sender, message) => { this.OnGlobalUserState?.Invoke(sender, message); }));

            this.Client = client;
            this.Client.OnMessageReceived += Client_OnMessageReceived;
            this.Client.OnConnect += Client_OnConnect;
        }

        private void Client_OnConnect(IrcMessage message)
        {
            this.Client.WriteLine("CAP REQ :twitch.tv/tags twitch.tv/commands");
            this.Client.WriteLine("JOIN #jtv");
            this.OnConnect?.Invoke();
        }

        private void Client_OnMessageReceived(IrcMessage message)
        {
            var parsedMessage = new ChatMessage(message.User, message.Middles.First(), message.Trailing, message.Tags);
            if (this.commandMap.ContainsKey(message.Command))
            {
                this.commandMap[message.Command](this, parsedMessage);
            }
            else
            {
                var bytes = Encoding.Default.GetBytes("Unknown command received: " + message.Command + "\r\n" + message.ToString());
                Console.OpenStandardError().Write(bytes, 0, bytes.Length);
                this.OnChatEvent?.Invoke(this, parsedMessage);
            }
        }
        public void Connect(string username, string token, bool useSSl = false)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("username");
            }
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException("token");
            }
            this.username = username;
            this.Client.Connect("irc.chat.twitch.tv", useSSl ? 6697 : 6667, useSSl, username, "oauth:" + token);
        }

        public void JoinChannel(string channel)
        {
            this.Client.WriteLine("JOIN #" + channel);
        }

        public void SendMessage(string channel, string content)
        {
            this.Client.WriteLine("PRIVMSG #" + channel + " :" + content);
        }

        public void SendWhisper(string user, string content)
        {
            this.Client.WriteLine("PRIVMSG #" + this.username + " :/w " + user + " " + content);
        }
    }
}
