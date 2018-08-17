using MemoryChallenge.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryChallenge
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start server...");
            try
            {
                ServerListener.StartListening();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }
        }
    }
}
