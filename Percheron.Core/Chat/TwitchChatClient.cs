using Percheron.Core.Irc;
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
    public class TwitchChatClient
    {
        private IDictionary<string, Action<ChatMessage>> commandMap;
        private string username;

        /// <summary>
        /// The internal IRC client used to manage the connection.
        /// </summary>
        public IIrcClient Client { get; private set; }

        public delegate void TwitchEvent(ChatMessage message);
        public delegate void Connected();

        /// <summary>
        /// Fired when a chat message is received. Fired for all events not otherwise covered, mainly used for debugging.
        /// </summary>
        public event TwitchEvent OnChatEvent;

        /// <summary>
        /// Fired when a message is sent to a channel you are in.
        /// </summary>
        public event TwitchEvent OnPrivMsg;
        /// <summary>
        /// Fired when a user joins a channel.
        /// </summary>
        public event TwitchEvent OnJoin;
        /// <summary>
        /// Fired when a user leaves a channel.
        /// </summary>
        public event TwitchEvent OnPart;
        /// <summary>
        /// Fired when a user's moderator status changes
        /// </summary>
        public event TwitchEvent OnMode;
        /// <summary>
        /// Fired when the server lists the users in a channel. (353)
        /// </summary>
        public event TwitchEvent OnNames;
        /// <summary>
        /// Fired when the server is finished listing the users in a channel. (366)
        /// </summary>
        public event TwitchEvent OnNamesComplete;
        /// <summary>
        /// Fired when a user's messages should be purged, such as when a user is banned or timed out.
        /// </summary>
        public event TwitchEvent OnClearChat;
        /// <summary>
        /// Fired when a single message should be purged.
        /// </summary>
        public event TwitchEvent OnClearMsg;
        /// <summary>
        /// Fired when a channel starts or stops hosting another channel.
        /// </summary>
        public event TwitchEvent OnHostTarget;
        /// <summary>
        /// Fired for general-use notices from the server, such as entering or exiting slow mode.
        /// </summary>
        public event TwitchEvent OnNotice;
        /// <summary>
        /// Fired when the server needs to shut down, requesting clients reconnect and rejoin channels.
        /// </summary>
        public event TwitchEvent OnReconnect;
        /// <summary>
        /// Fired when joining a channel or when the state changes, such as entering or exiting slow mode.
        /// </summary>
        public event TwitchEvent OnRoomState;
        /// <summary>
        /// Fired when a user triggers an event, such as subscribing.
        /// </summary>
        public event TwitchEvent OnUserNotice;
        /// <summary>
        /// Fired when joining a channel with information about your user state, such as name color.
        /// </summary>
        public event TwitchEvent OnUserState;
        /// <summary>
        /// Fired when logging in with information about your user state, such as name color.
        /// </summary>
        public event TwitchEvent OnGlobalUserState;

        /// <summary>
        /// Event fired once the user is connected and authenticated.
        /// </summary>
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
            this.commandMap = new Dictionary<string, Action<ChatMessage>>();
            this.commandMap.Add("PRIVMSG", new Action<ChatMessage>((message) => { this.OnPrivMsg?.Invoke(message); }));
            this.commandMap.Add("JOIN", new Action<ChatMessage>((message) => { this.OnJoin?.Invoke(message); }));
            this.commandMap.Add("PART", new Action<ChatMessage>((message) => { this.OnPart?.Invoke(message); }));
            this.commandMap.Add("MODE", new Action<ChatMessage>((message) => { this.OnMode?.Invoke(message); }));
            this.commandMap.Add("353", new Action<ChatMessage>((message) => { this.OnNames?.Invoke(message); }));
            this.commandMap.Add("366", new Action<ChatMessage>((message) => { this.OnNamesComplete?.Invoke(message); }));
            this.commandMap.Add("CLEARCHAT", new Action<ChatMessage>((message) => { this.OnClearChat?.Invoke(message); }));
            this.commandMap.Add("CLEARMSG", new Action<ChatMessage>((message) => { this.OnClearMsg?.Invoke(message); }));
            this.commandMap.Add("HOSTTARGET", new Action<ChatMessage>((message) => { this.OnHostTarget?.Invoke(message); }));
            this.commandMap.Add("NOTICE", new Action<ChatMessage>((message) => { this.OnNotice?.Invoke(message); }));
            this.commandMap.Add("RECONNECT", new Action<ChatMessage>((message) => { this.OnReconnect?.Invoke(message); }));
            this.commandMap.Add("ROOMSTATE", new Action<ChatMessage>((message) => { this.OnRoomState?.Invoke(message); }));
            this.commandMap.Add("USERNOTICE", new Action<ChatMessage>((message) => { this.OnUserNotice?.Invoke(message); }));
            this.commandMap.Add("USERSTATE", new Action<ChatMessage>((message) => { this.OnUserState?.Invoke(message); }));
            this.commandMap.Add("GLOBALUSERSTATE", new Action<ChatMessage>((message) => { this.OnGlobalUserState?.Invoke(message); }));

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
                this.commandMap[message.Command](parsedMessage);
            }
            else
            {
                var bytes = Encoding.Default.GetBytes("Unknown command received: " + message.Command + "\r\n" + message.ToString());
                Console.OpenStandardError().Write(bytes, 0, bytes.Length);
                this.OnChatEvent?.Invoke(parsedMessage);
            }
        }

        /// <summary>
        /// Connect the client to twitch's IRC chat server.
        /// </summary>
        /// <param name="username">The twitch username of the account to connect.</param>
        /// <param name="token">A valid oauth token from the twitch authentication service.</param>
        /// <param name="useSSl">Tells the client to connect with SSL. Defaults to false.</param>
        /// <exception cref="ArgumentNullException">If the username or token arguments are null or have a length of 0.</exception>
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

        /// <summary>
        /// Joins a twitch channel. This should be the name of a streamer.
        /// </summary>
        /// <param name="channel">The name of the streamer's channel to join. You do not need to prefix this with anything.</param>
        public void JoinChannel(string channel)
        {
            this.Client.WriteLine("JOIN #" + channel);
        }

        /// <summary>
        /// Sends a message to a channel. If you are not in that channel, the message will not be sent.
        /// </summary>
        /// <param name="channel">The name of the channel to send the message to.</param>
        /// <param name="content">The message to send.</param>
        public void SendMessage(string channel, string content)
        {
            this.Client.WriteLine("PRIVMSG #" + channel + " :" + content);
        }

        /// <summary>
        /// Sends a message to a user. If that user does not exist, the message will not be sent.
        /// </summary>
        /// <param name="user">The username of the recipient.</param>
        /// <param name="content">The message to send.</param>
        public void SendWhisper(string user, string content)
        {
            this.Client.WriteLine("PRIVMSG #" + this.username + " :/w " + user + " " + content);
        }
    }
}
