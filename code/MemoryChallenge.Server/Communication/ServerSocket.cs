using MemoryChallenge.Controllers;
using MemoryChallenge.Infrastructure;
using MemoryChallenge_Adapter.Common;
using MemoryChallenge_Adapter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MemoryChallenge.Common
{
    public static class ServerListener
    {
        private const int portNumer = 11000;

        // public static ChanelsContainer chanelsContainer = new ChanelsContainer();
        public static ServerController serverController = new ServerController();
        private static Socket serverListenerHandler;

        private static IPHostEntry ipHostInfo;
        private static IPAddress ipAddress;
        private static IPEndPoint localEndPoint;

        public static ManualResetEvent allDone = new ManualResetEvent(false);

        static ServerListener()
        {
            ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            ipAddress = ipHostInfo.AddressList[0];
            localEndPoint = new IPEndPoint(ipAddress, portNumer);

            serverListenerHandler = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public static void StartListening()
        {
            try
            {
                serverListenerHandler.Bind(localEndPoint);
                serverListenerHandler.Listen(10);

                while (true)
                {
                    allDone.Reset();

                    serverListenerHandler.BeginAccept(new AsyncCallback(AcceptCallback), serverListenerHandler);

                    allDone.WaitOne();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            allDone.Set();

            var listener = (Socket)ar.AsyncState;
            var clientSocket = listener.EndAccept(ar); 

            var chanel = new Chanel(clientSocket);

            Console.WriteLine("client connected: " + SocketLogger.ChanelInfo(chanel));

            clientSocket.BeginReceive(chanel.Buffer, 0, Chanel.BufferSize, 0, new AsyncCallback(ReadCallback), chanel);
        }

        private static void ReadCallback(IAsyncResult ar)
        {
            var chanel = (Chanel)ar.AsyncState;
            int bytesRead = chanel.ChanelHandler.EndReceive(ar);

            if (bytesRead > 0)
            {
                chanel.BufferState.AddRange(chanel.Buffer);

                var messageSizeBytes = chanel.Buffer.Take(4).ToArray();
                int messageSize = BitConverter.ToInt32(messageSizeBytes, 0);

                if (bytesRead < messageSize)
                {
                    chanel.ChanelHandler.BeginReceive(chanel.Buffer, 0, Chanel.BufferSize, 0, new AsyncCallback(ReadCallback), chanel);
                }
                else
                {
                    var message = Serializer.Deserialize<SocketMessage>(chanel.BufferState.Skip(4).ToArray());

                    var result = ReflectionHelper.InvokeFunction(serverController, message.CallbackFunction,
                        new object[] { chanel, message.Data, message.UserName });

                    Send(chanel, result);
                }
            }
        }

        private static void Send(Chanel chanel, object result)
        {
            var data = Serializer.Serialize(result);
            chanel.ChanelHandler.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), chanel);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            var chanel = (Chanel)ar.AsyncState;
            var bytesSent = chanel.ChanelHandler.EndSend(ar);

            Console.WriteLine("client disconected: " + SocketLogger.ChanelInfo(chanel));

            chanel.ChanelHandler.Shutdown(SocketShutdown.Both);
            chanel.ChanelHandler.Close();
        }
    }
}
