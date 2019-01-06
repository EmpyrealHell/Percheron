using Percheron.Core.Irc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Percheron.Core.Chat
{
    public class ChatClient
    {
        public IrcClient Client { get; private set; }
        public StreamWriter DebugStream { get; private set; }

        public delegate void MessageReceived(ChatMessage message);

        public event MessageReceived OnMessageReceived;

        public ChatClient(Stream debug = null)
        {
            this.Client = new IrcClient();
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
            this.Client.Connect("irc.chat.twitch.tv", useSSl ? 6697 : 6667, useSSl, username, "oauth:" + token);
        }
        /*
        public ChatClient(Stream debug = null)
        {
            if (debug != null)
            {
                this.DebugStream = new StreamWriter(debug, Encoding.UTF8);
            }
            else
            {
                this.DebugStream = new StreamWriter(Console.OpenStandardOutput(), Encoding.UTF8);
            }
            this.Client = new StandardIrcClient();
            this.Client.ClientInfoReceived += Client_ClientInfoReceived;
            this.Client.Connected += Client_Connected;
            this.Client.ConnectFailed += Client_ConnectFailed;
            this.Client.Disconnected += Client_Disconnected;
            this.Client.Error += Client_Error;
            this.Client.ErrorMessageReceived += Client_ErrorMessageReceived;
            this.Client.MotdReceived += Client_MotdReceived;
            this.Client.NetworkInformationReceived += Client_NetworkInformationReceived;
            this.Client.PingReceived += Client_PingReceived;
            this.Client.PongReceived += Client_PongReceived;
            this.Client.ProtocolError += Client_ProtocolError;
            this.Client.RawMessageReceived += Client_RawMessageReceived;
            this.Client.RawMessageSent += Client_RawMessageSent;
            this.Client.Registered += Client_Registered;
            this.Client.ServerBounce += Client_ServerBounce;
            this.Client.ValidateSslCertificate += Client_ValidateSslCertificate;
        }

        private void Client_ValidateSslCertificate(object sender, IrcValidateSslCertificateEventArgs e)
        {
            this.DebugStream.WriteLine("Validate SSL");
            this.DebugStream.WriteLine("\tCertificate: " + e.Certificate.ToString());
            this.DebugStream.WriteLine("\tChain: " + e.Chain.ToString());
            this.DebugStream.WriteLine("\tIs Valid?: " + e.IsValid);
            this.DebugStream.WriteLine("\tErrors: " + e.SslPolicyErrors);
            this.DebugStream.Flush();
        }

        private void Client_ServerBounce(object sender, IrcServerInfoEventArgs e)
        {
            this.DebugStream.WriteLine("Server bounced");
            this.DebugStream.WriteLine("\tTo:" + e?.Address + ":" + e?.Port);
            this.DebugStream.Flush();
        }

        private void Client_Registered(object sender, EventArgs e)
        {
            this.DebugStream.WriteLine("Registered");
            this.DebugStream.Flush();
        }

        private void Client_RawMessageSent(object sender, IrcRawMessageEventArgs e)
        {
            this.DebugStream.WriteLine("Message sent");
            this.DebugStream.WriteLine("\t" + e?.RawContent);
            this.DebugStream.Flush();
        }

        private void Client_RawMessageReceived(object sender, IrcRawMessageEventArgs e)
        {
            this.DebugStream.WriteLine("Message received");
            this.DebugStream.WriteLine("\tContent: " + e.Message.ToString());
            this.DebugStream.WriteLine("\tCommand: " + e.Message.Command);
            this.DebugStream.WriteLine("\tPrefix: " + e.Message.Prefix);
            this.DebugStream.WriteLine("\tSource: " + e.Message.Source?.Name);
            this.DebugStream.WriteLine("\tParameters: " + string.Join("; ", e.Message.Parameters));
            this.DebugStream.WriteLine("\tRaw:" + e?.RawContent);
            this.DebugStream.Flush();
        }

        private void Client_ProtocolError(object sender, IrcProtocolErrorEventArgs e)
        {
            this.DebugStream.WriteLine("Client error");
            this.DebugStream.WriteLine("\tCode: " + e?.Code);
            this.DebugStream.WriteLine("\tMessage: " + e?.Message);
            this.DebugStream.WriteLine("\tParameters: " + string.Join("; ", e?.Parameters));
            this.DebugStream.Flush();
        }

        private void Client_PongReceived(object sender, IrcPingOrPongReceivedEventArgs e)
        {
            this.DebugStream.WriteLine("Pong received");
            this.DebugStream.WriteLine("\tFrom: " + e?.Server);
            this.DebugStream.Flush();
        }

        private void Client_PingReceived(object sender, IrcPingOrPongReceivedEventArgs e)
        {
            this.DebugStream.WriteLine("> PING :" + e?.Server);
            this.Client.SendRawMessage("\tPONG :" + e?.Server);
            this.DebugStream.Flush();
        }

        private void Client_NetworkInformationReceived(object sender, IrcCommentEventArgs e)
        {
            this.DebugStream.WriteLine("Network information received");
            this.DebugStream.WriteLine("\tComment: " + e?.Comment);
            this.DebugStream.Flush();
        }

        private void Client_MotdReceived(object sender, EventArgs e)
        {
            this.DebugStream.WriteLine("MotD received");
            this.DebugStream.Flush();
        }

        private void Client_ErrorMessageReceived(object sender, IrcErrorMessageEventArgs e)
        {
            this.DebugStream.WriteLine("Server error");
            this.DebugStream.WriteLine("\tError: " + e?.Message);
            this.DebugStream.Flush();
        }

        private void Client_Error(object sender, IrcErrorEventArgs e)
        {
            this.DebugStream.WriteLine("Client error");
            this.DebugStream.WriteLine("\tError: " + e?.Error);
            this.DebugStream.Flush();
        }

        private void Client_Disconnected(object sender, EventArgs e)
        {
            this.DebugStream.WriteLine("Disconnected");
            this.DebugStream.Flush();
        }

        private void Client_ConnectFailed(object sender, IrcErrorEventArgs e)
        {
            this.DebugStream.WriteLine("Connection failed");
            this.DebugStream.WriteLine("\tError: " + e?.Error);
            this.DebugStream.Flush();
        }

        private void Client_Connected(object sender, EventArgs e)
        {
            this.DebugStream.WriteLine("Connected, sending login information");
            this.Client.SendRawMessage("CAP REQ :twitch.tv/tags twitch.tv/commands");
            this.Client.Channels.Join("#jtv");
            this.DebugStream.Flush();
        }

        private void Client_ClientInfoReceived(object sender, EventArgs e)
        {
            this.DebugStream.WriteLine("Client info received");
            this.DebugStream.Flush();
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
            var registration = new IrcUserRegistrationInfo();
            registration.NickName = username;
            registration.RealName = username;
            registration.UserName = username;
            registration.Password = "oauth:" + token;
            this.Client.Connect("irc.chat.twitch.tv", useSSl ? 6697 : 6667, useSSl, registration);
        }

        /// <summary>
        /// Sends a message to a specific channel.
        /// </summary>
        /// <param name="channel">The channel to send to. This is typically the name of a streamer to target their stream chat.</param>
        /// <param name="message">The message text to send.</param>
        /// <exception cref="ArgumentNullException">If the channel or message arguments are null or have a length of 0.</exception>
        public void SendMessage(string channel, string message)
        {
            if (string.IsNullOrEmpty(channel))
            {
                throw new ArgumentNullException("channel");
            }
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException("message");
            }
            this.Client.LocalUser.SendMessage("#" + channel, message);
        }

        /// <summary>
        /// Connect to a twitch stream channel.
        /// </summary>
        /// <param name="channel">The name of the channel. This is typically the name of a streamer to target their stream chat.</param>
        /// <exception cref="ArgumentNullException">If the channel is null or has a length of 0.</exception>
        public void JoinChannel(string channel)
        {
            if (string.IsNullOrEmpty(channel))
            {
                throw new ArgumentNullException("channel");
            }

            if (channel[0] != '#')
            {
                channel = "#" + channel;
            }
            this.Client.Channels.Join(channel);
        }
        */
    }
}
