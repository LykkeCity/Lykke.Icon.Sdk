using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
    public interface RpcConverter<T>
    {
        T ConvertTo(RpcItem @object);
        RpcItem ConvertFrom(T @object);
    }

    public interface RpcConverterFactory
    {
        RpcConverter<T> Create<T>();
    }
}
