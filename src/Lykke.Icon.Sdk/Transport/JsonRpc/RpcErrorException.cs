using System;
using JetBrains.Annotations;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
    [PublicAPI]
    public class RpcErrorException : Exception
    {
        public RpcErrorException()
        {
            
        }

        public RpcErrorException(long code, string message) 
            : base(message)
        {
            Code = code;
        }

        public long Code { get; }
    }
}