using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{

    public abstract class RpcItem
    {

        public abstract bool IsEmpty();

        public virtual RpcObject ToObject()
        {
            if (this is RpcObject) return (RpcObject)this;
            throw new RpcValueException("This item can not be converted to RpcObject");
        }

        public virtual RpcArray ToArray()
        {
            if (this is RpcArray) return (RpcArray)this;
            throw new RpcValueException("This item can not be converted to RpcValue");
        }

        public virtual RpcValue ToValue()
        {
            if (this is RpcValue) return (RpcValue)this;
            throw new RpcValueException("This item can not be converted to RpcValue");
        }

        public virtual String ToString()
        {
            return ToValue().ToString();
        }

        public virtual BigInteger ToInteger()
        {
            return asValue().asInteger();
        }

        byte[] ToByteArray()
        {
            return asValue().asByteArray();
        }

        Boolean ToBoolean()
        {
            return asValue().asBoolean();
        }

        Address ToAddress()
        {
            return asValue().asAddress();
        }

        Bytes ToBytes()
        {
            return ToValue().ToBytes();
        }
    }

    public class RpcValueException : ArgumentException
    {
        RpcValueException(String message) : base(message)
        {
        }
    }
}
