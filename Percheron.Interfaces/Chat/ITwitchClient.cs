using System;
using System.Collections.Generic;
using System.Text;

namespace Percheron.Interfaces.Chat
{
    public delegate void TwitchEvent(ITwitchClient sender, ChatMessage message);
    public delegate void Connected();

    public interface ITwitchClient
    {

        /// <summary>
        /// The internal IRC client used to manage the connection.
        /// </summary>
        IIrcClient Client { get; }

        /// <summary>
        /// Fired when a chat message is received. Fired for all events not otherwise covered, mainly used for debugging.
        /// </summary>
        event TwitchEvent OnChatEvent;

        /// <summary>
        /// Fired when a message is sent to a channel you are in.
        /// </summary>
        event TwitchEvent OnPrivMsg;
        /// <summary>
        /// Fired when a user joins a channel.
        /// </summary>
        event TwitchEvent OnJoin;
        /// <summary>
        /// Fired when a user leaves a channel.
        /// </summary>
        event TwitchEvent OnPart;
        /// <summary>
        /// Fired when a user's moderator status changes
        /// </summary>
        event TwitchEvent OnMode;
        /// <summary>
        /// Fired when the server lists the users in a channel. (353)
        /// </summary>
        event TwitchEvent OnNames;
        /// <summary>
        /// Fired when the server is finished listing the users in a channel. (366)
        /// </summary>
        event TwitchEvent OnNamesComplete;
        /// <summary>
        /// Fired when a user's messages should be purged, such as when a user is banned or timed out.
        /// </summary>
        event TwitchEvent OnClearChat;
        /// <summary>
        /// Fired when a single message should be purged.
        /// </summary>
        event TwitchEvent OnClearMsg;
        /// <summary>
        /// Fired when a channel starts or stops hosting another channel.
        /// </summary>
        event TwitchEvent OnHostTarget;
        /// <summary>
        /// Fired for general-use notices from the server, such as entering or exiting slow mode.
        /// </summary>
        event TwitchEvent OnNotice;
        /// <summary>
        /// Fired when the server needs to shut down, requesting clients reconnect and rejoin channels.
        /// </summary>
        event TwitchEvent OnReconnect;
        /// <summary>
        /// Fired when joining a channel or when the state changes, such as entering or exiting slow mode.
        /// </summary>
        event TwitchEvent OnRoomState;
        /// <summary>
        /// Fired when a user triggers an event, such as subscribing.
        /// </summary>
        event TwitchEvent OnUserNotice;
        /// <summary>
        /// Fired when joining a channel with information about your user state, such as name color.
        /// </summary>
        event TwitchEvent OnUserState;
        /// <summary>
        /// Fired when logging in with information about your user state, such as name color.
        /// </summary>
        event TwitchEvent OnGlobalUserState;

        /// <summary>
        /// Event fired once the user is connected and authenticated.
        /// </summary>
        event Connected OnConnect;

        /// <summary>
        /// Connect the client to twitch's IRC chat server.
        /// </summary>
        /// <param name="username">The twitch username of the account to connect.</param>
        /// <param name="token">A valid oauth token from the twitch authentication service.</param>
        /// <param name="useSSl">Tells the client to connect with SSL. Defaults to false.</param>
        /// <exception cref="ArgumentNullException">If the username or token arguments are null or have a length of 0.</exception>
        void Connect(string username, string token, bool useSSl = false);

        /// <summary>
        /// Joins a twitch channel. This should be the name of a streamer.
        /// </summary>
        /// <param name="channel">The name of the streamer's channel to join. You do not need to prefix this with anything.</param>
        void JoinChannel(string channel);

        /// <summary>
        /// Sends a message to a channel. If you are not in that channel, the message will not be sent.
        /// </summary>
        /// <param name="channel">The name of the channel to send the message to.</param>
        /// <param name="content">The message to send.</param>
        void SendMessage(string channel, string content);

        /// <summary>
        /// Sends a message to a user. If that user does not exist, the message will not be sent.
        /// </summary>
        /// <param name="user">The username of the recipient.</param>
        /// <param name="content">The message to send.</param>
        void SendWhisper(string user, string content);
    }
}
