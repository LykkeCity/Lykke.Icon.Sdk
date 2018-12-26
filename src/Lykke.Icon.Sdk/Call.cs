using System;
using JetBrains.Annotations;
using Lykke.Icon.Sdk.Transport.JsonRpc;

namespace Lykke.Icon.Sdk
{
    [PublicAPI]
    public partial class Call
    {
        private readonly RpcObject _properties;
        private readonly Type _responseType;

        public Call(
            RpcObject properties,
            Type responseType)
        {
            _properties = properties;
            _responseType = responseType;
        }


        public RpcObject GetProperties()
        {
            return _properties;
        }

        public Type GetResponseType()
        {
            return _responseType;
        }
    }
    
    [PublicAPI]
    public class Call<T> : Call
    {
        public Call(RpcObject properties) 
            : base(properties, typeof(T))
        {
            
        }
    }
}