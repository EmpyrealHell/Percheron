using Percheron.Core.Chat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Percheron.Core.Irc
{
    /// <summary>
    /// A low-level IRC client. Allow you to send raw messages, and receive messages from the server broken up according to RFC1459. Also has limited processing of numeric commands, and automatically responds to ping messages.
    /// </summary>
    public class IrcClient : IIrcClient, IDisposable
    {
        public event IrcMessageEvent OnMessageReceived;
        public event IrcMessageEvent OnCommandReceived;
        public event IrcMessageEvent OnConnect;
        public event IrcMessageEvent OnPing;

        private static readonly Regex numericCommandChecker = new Regex("[0-9]{3}");

        private INetworkClient client;
        private StreamReader reader;
        private StreamWriter writer;

        private Task serverReadTask;
        private CancellationTokenSource cancelToken;

        private StreamWriter debugStream;

        private IDictionary<string, Action<IrcMessage>> commandMap;

        /// <summary>
        /// Creates an IRC client. All messages sent and received will be logged to the console, in addition to coming through the event system.
        /// </summary>
        public IrcClient() : this(Console.OpenStandardOutput())
        {
        }

        /// <summary>
        /// Creates an IRC client. All messages sent and received will be logged to the specified stream, in addition to coming through the event system. If stream is null, no messages will be logged.
        /// </summary>
        public IrcClient(Stream debugStream)
        {
            if (debugStream != null)
            {
                this.debugStream = new StreamWriter(debugStream);
                this.debugStream.AutoFlush = true;
            }
            this.commandMap = new Dictionary<string, Action<IrcMessage>>();
            this.commandMap.Add("001", this.DoConnect);
            this.commandMap.Add("PING", this.DoPing);
        }

        private void DoConnect(IrcMessage message)
        {
            this.OnConnect?.Invoke(message);
        }

        private void DoPing(IrcMessage message)
        {
            this.WriteLine("PONG" + message.Params);
            this.OnPing?.Invoke(message);
        }

        private void ServerReadAction()
        {
            while (!this.cancelToken.IsCancellationRequested)
            {
                try
                {
                    var line = this.reader.ReadLine();
                    var message = new IrcMessage(line);
                    this.debugStream?.WriteLine("> " + message.ToString());
                    if (!line.Equals(message.ToString()))
                    {
                        this.debugStream?.WriteLine("Parse Error: " + line);
                    }

                    if (this.commandMap.ContainsKey(message.Command))
                    {
                        this.commandMap[message.Command](message);
                    }

                    if (numericCommandChecker.IsMatch(message.Command))
                    {
                        this.OnCommandReceived?.Invoke(message);
                    }
                    else
                    {
                        this.OnMessageReceived?.Invoke(message);
                    }
                }
                catch
                {
                }
            }
        }

        public void Connect(string url, int port, bool useSSL, string user, string pass)
        {
            if (useSSL)
            {
                this.Connect(new SslNetworkClient(url, port), user, pass);
            }
            else
            {
                this.Connect(new TcpNetworkClient(url, port), user, pass);
            }
        }

        public void Connect(INetworkClient client, string user, string pass)
        {
            this.client = client;
            this.client.SendTimeout = (int)new TimeSpan(0, 6, 0).TotalMilliseconds;
            this.client.ReceiveTimeout = (int)new TimeSpan(0, 6, 0).TotalMilliseconds;
            var encoding = new UTF8Encoding(false);
            this.reader = new StreamReader(this.client.GetNetworkStream(), encoding);
            this.writer = new StreamWriter(this.client.GetNetworkStream(), encoding);
            this.writer.NewLine = "\r\n";
            this.writer.AutoFlush = true;

            this.WriteLine("PASS " + pass);
            this.WriteLine("NICK " + user.ToLower());

            this.cancelToken = new CancellationTokenSource();
            this.serverReadTask = new Task(this.ServerReadAction, this.cancelToken.Token, TaskCreationOptions.LongRunning);
            this.serverReadTask.Start();
        }

        public void WriteLine(string message)
        {
            this.debugStream?.WriteLine("< " + message);
            this.writer.WriteLine(message);
        }

        public void Close()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if (this.cancelToken != null)
            {
                this.cancelToken.Cancel();
            }
            if (this.debugStream != null)
            {
                this.debugStream.Close();
                this.debugStream.Dispose();
            }
            if (this.writer != null)
            {
                this.writer.Close();
                this.writer.Dispose();
            }

            if (this.reader != null)
            {
                this.reader.Close();
                this.reader.Dispose();
            }

            if (this.client != null)
            {
                this.client.Close();
                this.client.Dispose();
            }
        }
    }
}
