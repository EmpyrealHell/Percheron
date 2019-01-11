using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Percheron.Core.Irc
{
    public interface INetworkClient : IDisposable
    {
        int SendTimeout { set; }
        int ReceiveTimeout { set; }
        void Close();
        Stream GetNetworkStream();
    }
}
