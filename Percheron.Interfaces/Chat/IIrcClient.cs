using Percheron.Interfaces.Chat;
using System;
using System.Collections.Generic;
using System.Text;

namespace Percheron.Interfaces.Chat
{
    public delegate void IrcMessageEvent(IrcMessage message);

    public interface IIrcClient
    {
        /// <summary>
        /// This event is fired every time a message is received from the server you are connected to, unless that message contains a numeric command.
        /// </summary>
        event IrcMessageEvent OnMessageReceived;
        /// <summary>
        /// This event is fired every time a message is received that contains a numeric command.
        /// </summary>
        event IrcMessageEvent OnCommandReceived;
        /// <summary>
        /// This event is fired when the server initially responds after a connection is established.
        /// </summary>
        event IrcMessageEvent OnConnect;
        /// <summary>
        /// This event is fired when the server sends a PING command, after the client has responded with the appropriate PONG command.
        /// </summary>
        event IrcMessageEvent OnPing;

        /// <summary>
        /// Connects to an IRC server.
        /// </summary>
        /// <param name="url">The url of the server to connect to.</param>
        /// <param name="port">The port to connect to on the server.</param>
        /// <param name="useSSL">Whether to connect with SSL.</param>
        /// <param name="user">The username to pass to the server on connect.</param>
        /// <param name="pass">The password to pass to the server on connect.</param>
        void Connect(string url, int port, bool useSSL, string user, string pass);

        /// <summary>
        /// Writes a message to the IRC server.
        /// </summary>
        /// <param name="message">The message to send. This will not be processed in any way.</param>
        void WriteLine(string message);

        /// <summary>
        /// Closes the connection to the IRC server, and all related streams.
        /// </summary>
        void Close();
    }
}
