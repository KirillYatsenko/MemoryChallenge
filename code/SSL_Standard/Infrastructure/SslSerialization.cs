
using Newtonsoft.Json;
using SSL_Standard.Socket;
using System;
using System.Linq;
using System.Text;

namespace SSL_Standard.Infrastructure
{
    public static class SslSerialization
    {
        /// <summary>
        /// Convert json string to class instance
        /// </summary>
        /// <typeparam name="T">object type</typeparam>
        /// <param name="json"></param>
        /// <returns>class instance</returns>
        public static T JsonToObject<T>(object jsonObject)
        {
            return JsonConvert.DeserializeObject<T>(jsonObject.ToString());
        }

        private static string MessageToJson(SslMessage message)
        {
            return JsonConvert.SerializeObject(message);
        }

        internal static byte[] MessageToByteArray(SslMessage message)
        {
            var dataJson = MessageToJson(message);
            var messageBytes = Encoding.ASCII.GetBytes(dataJson);
            var messageSize = BitConverter.GetBytes(messageBytes.Length + 4);

            return messageSize.Concat(messageBytes).ToArray();
        }

        private static string BytesToJson(byte[] data)
        {
            return Encoding.Default.GetString(data);
        }

        internal static T BytesToObject<T>(byte[] data)
        {
            var json = BytesToJson(data);
            return JsonConvert.DeserializeObject<T>(json);
        }

      
    }
}