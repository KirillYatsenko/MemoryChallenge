using MemoryChallenge.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MemoryChallenge.Infrastructure
{
    public static class SocketLogger
    {

        public static string ChanelInfo(Chanel chanel)
        {
            var remoteIpEndPoint = chanel.ChanelHandler.RemoteEndPoint as IPEndPoint;
            var info = $"client ip: {remoteIpEndPoint.Address} on port number: {remoteIpEndPoint.Port}";

            return info;
        }
    }
}
