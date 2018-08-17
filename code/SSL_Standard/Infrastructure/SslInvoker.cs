using System;
using System.Collections.Generic;
using System.Text;

namespace SSL_Standard.Infrastructure
{
    internal static class SslInvoker
    {
        public static void InvokeFunction(object classInstance, string methodName, object[] parameters)
        {
            var classType = classInstance.GetType();
            var methodInfo = classType.GetMethod(methodName);

            methodInfo.Invoke(classInstance, parameters);
        }
    }
}

