using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Percheron.Core.Irc
{
    public class SslTcpClient : TcpClient
    {
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }
            Console.Error.WriteLine("Certificate Error: " + sslPolicyErrors);
            return false;
        }

        public SslStream SslStream { get; private set; }

        public SslTcpClient() : base()
        {
            throw new NotImplementedException("This constructor is not implemented, use the SslTcpClient(string hostname, int port) constructor instead.");
        }

        public SslTcpClient(AddressFamily family) : base(family)
        {
            throw new NotImplementedException("This constructor is not implemented, use the SslTcpClient(string hostname, int port) constructor instead.");
        }

        public SslTcpClient(IPEndPoint localEP) : base(localEP)
        {
            throw new NotImplementedException("This constructor is not implemented, use the SslTcpClient(string hostname, int port) constructor instead.");
        }

        public SslTcpClient(string hostname, int port) : base(hostname, port)
        {
            this.AuthenticateSSL(hostname);
        }

        public void AuthenticateSSL(string hostname)
        {
            this.SslStream = new SslStream(this.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
            try
            {
                this.SslStream.AuthenticateAsClient(hostname);
            }
            catch (AuthenticationException e)
            {
                Console.Error.WriteLine("Exception attempting to authenticate SSL stream: " + e.Message);
                if (e.InnerException != null)
                {
                    Console.Error.WriteLine("Inner Exception: " + e.InnerException.Message);
                }
                this.Close();
            }
        }
    }
}
