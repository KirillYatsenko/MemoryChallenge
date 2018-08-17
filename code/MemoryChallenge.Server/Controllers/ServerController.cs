using MemoryChallenge.Common;
using MemoryChallenge.Infrastructure;
using MemoryChallenge_Adapter.FunctionsNames;
using MemoryChallenge_Adapter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MemoryChallenge.Controllers
{
    public class ServerController
    {
       // private ChanelsContainer container;

        public ServerController()
        {
          //  container = _container;
        }

        public SocketMessage StartGame(Chanel chanel, object data, string userName)
        {
            var gameZone = GameCreator.StartGame();

            Console.WriteLine($"{userName} Started game");

            return new SocketMessage(ClientFunctionsNames.SetGameZone.ToString(), gameZone);
        }

        public SocketMessage ContinueGame(Chanel chanel, object data, string userName)
        {
            int level = (int)data;
            var gameZone = GameCreator.GenerateZone(level);

            Console.WriteLine($"{userName} on {level} level ");

            return new SocketMessage(ClientFunctionsNames.SetGameZone.ToString(), gameZone);
        }
    }
}
