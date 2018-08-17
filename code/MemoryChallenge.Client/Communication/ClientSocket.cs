using MemoryChallenge.Common;
using MemoryChallenge_Adapter.Common;
using MemoryChallenge_Adapter.FunctionsNames;
using MemoryChallenge_Adapter.Model;
using MemoryChallenge_Client.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Sockets = System.Net.Sockets;


namespace MemoryChallenge_Client.Socket
{
    public class ClientSocketHandler
    {
        private Sockets.Socket clientSocket;

        private IPHostEntry ipHostInfo;
        private IPAddress ipAddress;
        private IPEndPoint remoteEndPoint;

        private ClientController clientController;

        public ClientSocketHandler(ClientController controller)
        {
            ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            ipAddress = ipHostInfo.AddressList[0];
            remoteEndPoint = new IPEndPoint(ipAddress, 11000);

            clientController = controller;
        }

        public void SentMessage(ServerFunctionsNames serverFunctionName,object data = null, string UserName = "Default")
        {
            clientSocket = new Sockets.Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                clientSocket.Connect(remoteEndPoint);

                var buffer = new byte[2048];

                byte[] messageData = Serializer.Serialize(new SocketMessage(serverFunctionName.ToString(),UserName ,data));
                var messageSize = BitConverter.GetBytes(messageData.Count() + 4);

                var message = messageSize.Concat(messageData).ToArray();

                clientSocket.Send(message);

                var recievedBytesCount = clientSocket.Receive(buffer);
                var recievedMessage = Serializer.Deserialize<SocketMessage>(buffer);
                ReflectionHelper.InvokeFunction(clientController, recievedMessage.CallbackFunction, new object[] { recievedMessage.Data });

                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to send message to serwer");
            }

        }
    }
}
