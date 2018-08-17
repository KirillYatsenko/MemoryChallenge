using SSL_Standard.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using NetSockets = System.Net.Sockets;

namespace SSL_Standard.Socket
{
    public class Sender
    {
        private Communicator sslSocket;

        private int portForReplaying;

        public int Port
        {
            get
            {
                return portForReplaying;
            }
        }

        public string IP { get; set; }

        public Sender(int port_replaying, NetSockets.Socket net_socket, Communicator ssl_communicator)
        {
            portForReplaying = port_replaying;
            sslSocket = ssl_communicator;

            IP = (net_socket.RemoteEndPoint as IPEndPoint).Address.ToString();
        }

        public void Reply(object data, string method_name = null)
        {
            sslSocket.Send(data, IP, portForReplaying,method_name);
        }
    }
}
