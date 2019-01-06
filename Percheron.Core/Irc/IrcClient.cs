using Percheron.Core.Chat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Percheron.Core.Irc
{
    public class IrcClient : IDisposable
    {
        public delegate void IrcMessageEvent(ChatMessage message);

        public event IrcMessageEvent OnMessageReceived;

        private TcpClient client;
        private StreamReader reader;
        private StreamWriter writer;

        public IrcClient()
        {

        }

        public void Connect(string url, int port, bool useSSL, string user, string pass)
        {
            Stream clientStream;
            if (useSSL)
            {
                var sslClient = new SslTcpClient(url, port);
                clientStream = sslClient.SslStream;
                this.client = sslClient;
            }
            else
            {
                this.client = new TcpClient(url, port);
                clientStream = this.client.GetStream();
            }
            var encoding = new UTF8Encoding(false);
            this.reader = new StreamReader(clientStream, encoding);
            this.writer = new StreamWriter(clientStream, encoding);
            this.writer.NewLine = "\r\n";
            this.writer.AutoFlush = true;

            Console.WriteLine("< PASS " + pass);
            this.writer.WriteLine("PASS " + pass);
            Console.WriteLine("< NICK " + user.ToLower());
            this.writer.WriteLine("NICK " + user.ToLower());
            while (!this.reader.EndOfStream)
            {
                Console.WriteLine("> " + this.reader.ReadLine());
            }
        }

        public void Close()
        {
            this.Dispose();
        }

        public void Dispose()
        {
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

            if (this.client != null && this.client.Connected)
            {
                this.client.Close();
                this.client.Dispose();
            }
        }
    }
}
