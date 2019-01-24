using Percheron.Interfaces.Chat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Percheron.Core.Chat
{
    public class TcpNetworkClient : TcpClient, INetworkClient
    {
        public TcpNetworkClient() : base()
        {
        }

        public TcpNetworkClient(AddressFamily family) : base(family)
        {
        }

        public TcpNetworkClient(IPEndPoint localEP) : base(localEP)
        {
        }

        public TcpNetworkClient(string hostname, int port) : base(hostname, port)
        {
        }

        public Stream GetNetworkStream()
        {
            return this.GetStream();
        }
    }
}
