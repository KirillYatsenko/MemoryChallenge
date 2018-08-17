using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SSL_Standard.Infrastructure;
using NetSockets = System.Net.Sockets;

namespace SSL_Standard.Socket
{
    public class Communicator
    {
        public delegate void OnRecieveDelegate(Sender sender, object data);

        public delegate void OnSendDelegate(Communicator sender);

        /// <summary>
        /// Event occured when socket recieve data
        /// </summary>
        public event OnRecieveDelegate OnRecieveEvent;

        /// <summary>
        /// Event occured when socket send data
        /// </summary>
        public event OnSendDelegate OnSendEvent;

        private IPHostEntry _ipHostInfo;
        private IPAddress _ipAddress;
        private IPEndPoint _remoteEP;
        private int _listeningPort;

        private System.Net.Sockets.Socket _listener;

        private object _controller = null;

        /// <summary>
        /// socket ip address
        /// </summary>
        public string IP
        {
            get
            {
                return _ipHostInfo.AddressList[0].ToString();
            }
        }

        /// <summary>
        /// SSL Socket constructor 
        /// </summary>
        /// <param name="local_port_number">listening host port</param>
        /// <param name="_controler">api of the host</param>
        public Communicator(int? local_port_number  = null, object controller = null)
        {
            _controller = controller;

            if(local_port_number == null)
            {
                _listeningPort = PortFinder.GetAvaiblePort(1000);
            }
            else
            {
                _listeningPort = (int)local_port_number;
            }

            _ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            _ipAddress = _ipHostInfo.AddressList[0];
            _listener = new NetSockets.Socket(_ipAddress.AddressFamily, NetSockets.SocketType.Stream,
              NetSockets.ProtocolType.Tcp);
            IPEndPoint localEndPoint = new IPEndPoint(_ipAddress, _listeningPort);

            _listener.Bind(localEndPoint);

            Task.Factory.StartNew(startListening);
        }

        /// <summary>
        /// Send data to endpoint via tcp protocol
        /// </summary>
        /// <param name="data">data to be send</param>
        /// <param name="endpoint_ip">ip of the endpoint</param>
        /// <param name="endpoint_port_number">port of the endpoint</param>
        /// <param name="method_name">invoke endpoint, if endpoint host provided controller</param>
        public void Send(object data, string endpoint_ip, int endpoint_port_number, string method_name = null)
        {
            var ip = IPAddress.Parse(endpoint_ip);
            var remoteEP = new IPEndPoint(ip, endpoint_port_number);
            var sender = new NetSockets.Socket(ip.AddressFamily, NetSockets.SocketType.Stream,
                NetSockets.ProtocolType.Tcp);

            sender.Connect(remoteEP);

            var sslMessage = new SslMessage(data, _listeningPort, method_name);
            var message = SslSerialization.MessageToByteArray(sslMessage);
            int bytesSend = sender.Send(message);

            sender.Shutdown(NetSockets.SocketShutdown.Both);
            sender.Close();

            // OnSendEvent(this);
        }

        private const int BUFFER_SIZE = 65536;

        private void startListening()
        {
            byte[] data = new byte[BUFFER_SIZE];

            _listener.Listen(10);

            while (true)
            {
                var clientSocket = _listener.Accept();
                int bytesRecieve = clientSocket.Receive(data);

                if (bytesRecieve > 0)
                {
                    byte[] tempData = new byte[2048];

                    while (!isFullData(bytesRecieve, data))
                    {
                        bytesRecieve += clientSocket.Receive(tempData);
                        data.ToList().AddRange(tempData);
                    };

                    var sslMessage = SslSerialization.BytesToObject<SslMessage>(data.Skip(4).ToArray());
                    var sender = new Sender(sslMessage.PortForReplaying, clientSocket, this);

                    clientSocket.Shutdown(NetSockets.SocketShutdown.Both);
                    clientSocket.Close();

                    if (sslMessage.CallbackFunction != null && _controller != null)
                    {
                        SslInvoker.InvokeFunction(_controller, sslMessage.CallbackFunction, new object[] { sender, sslMessage.Data });
                    }

                    data = new byte[BUFFER_SIZE];
                  //  OnRecieveEvent(sender, sslMessage.Data);
                }
            }
        }

        private bool isFullData(int bytes_read, byte[] data)
        {
            var messageSizeBytes = data.Take(4).ToArray();
            int messageSize = BitConverter.ToInt32(messageSizeBytes, 0);

            if (bytes_read != messageSize)
            {
                return false;
            }

            return true;
        }
    }
}