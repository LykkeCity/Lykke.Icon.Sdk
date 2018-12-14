using Lykke.Icon.Sdk.Data;
using System;
using System.Numerics;

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
            return ToValue().ToInteger();
        }

        public byte[] ToByteArray()
        {
            return ToValue().ToByteArray();
        }

        public Boolean ToBoolean()
        {
            return ToValue().ToBoolean();
        }

        public Address ToAddress()
        {
            return ToValue().ToAddress();
        }

        public Bytes ToBytes()
        {
            return ToValue().ToBytes();
        }
    }

    public class RpcValueException : ArgumentException
    {
        public RpcValueException(String message) : base(message)
        {
        }
    }
}
