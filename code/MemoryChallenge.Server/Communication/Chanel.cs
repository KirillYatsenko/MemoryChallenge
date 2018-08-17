using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MemoryChallenge.Common
{
    public class Chanel
    {
        public static int BufferSize = 1024;

        public byte[] Buffer = new byte[BufferSize];
        public List<byte> BufferState = new List<byte>();

        public Socket ChanelHandler { get; set; }

        public Chanel(Socket handler)
        {
            this.ChanelHandler = handler;
        }
    }
}
