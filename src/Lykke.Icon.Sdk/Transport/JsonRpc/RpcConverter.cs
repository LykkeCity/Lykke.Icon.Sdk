using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
    public interface RpcConverter<T>
    {
        T convertTo(RpcItem @object);
        RpcItem convertFrom(T @object);

    }

    public interface RpcConverterFactory
    {
        RpcConverter<T> Create(T type);
    }
}
