using System;
using System.Collections.Generic;
using System.Text;

namespace SSL_Standard.Socket
{
    public class SslMessage
    {
        public int PortForReplaying { get; set; }
        public string CallbackFunction { get; set; }
        public object Data{ get; set; }

        public SslMessage(object data, int port, string callback_method = null)
        {
            PortForReplaying = port;
            Data = data;
            CallbackFunction = callback_method;
        }
    }
}
