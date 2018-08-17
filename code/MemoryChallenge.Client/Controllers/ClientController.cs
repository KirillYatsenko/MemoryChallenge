using MemoryChallenge_Adapter.FunctionsNames;
using MemoryChallenge_Adapter.Game;
using MemoryChallenge_Client.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryChallenge_Client.Controllers
{
    public class ClientController
    {
        public delegate void ClientControllerDel(object data);

        public event ClientControllerDel ConnectionEstablishedEvent;
        public event ClientControllerDel ChangeGameZoneEvent;
        public event ClientControllerDel ErrorOccuredEvent;

        private string userName { get; set; }

        private ClientSocketHandler clientSocket;

        public ClientController(string _userName = null)
        {
            clientSocket = new ClientSocketHandler(this);
            userName = _userName;
        }

        public void StartGame()
        {
            try
            {
                clientSocket.SentMessage(ServerFunctionsNames.StartGame, null,userName);
            }
            catch (Exception ex)
            {
                ErrorOccuredEvent(ex.Message);
            }
        }

        public void ContinueGame(int level)
        {
            try
            {
                clientSocket.SentMessage(ServerFunctionsNames.ContinueGame, level, userName);
            }
            catch(Exception ex)
            {
                ErrorOccuredEvent(ex.Message);
            }
        }

        // Events
        public void ConnectionEstablished(int id)
        {
            ConnectionEstablishedEvent(id);
        }

        public void SetGameZone(object data)
        {
            ChangeGameZoneEvent(data);
        }

    }
}
