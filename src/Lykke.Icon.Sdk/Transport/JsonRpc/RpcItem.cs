using System;
using System.Numerics;
using Lykke.Icon.Sdk.Data;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
    public abstract class RpcItem
    {
        public abstract bool IsEmpty();

        public virtual RpcObject ToObject()
        {
            if (this is RpcObject)
            {
                return (RpcObject)this;
            }
            
            throw new RpcValueException("This item can not be converted to RpcObject");
        }

        public virtual RpcArray ToArray()
        {
            if (this is RpcArray)
            {
                return (RpcArray)this;
            }
            
            throw new RpcValueException("This item can not be converted to RpcValue");
        }

        public virtual RpcValue ToValue()
        {
            if (this is RpcValue)
            {
                return (RpcValue)this;
            }
            
            throw new RpcValueException("This item can not be converted to RpcValue");
        }

        public virtual BigInteger ToInteger()
        {
            return ToValue().ToInteger();
        }

        public virtual byte[] ToByteArray()
        {
            return ToValue().ToByteArray();
        }

        public virtual bool ToBoolean()
        {
            return ToValue().ToBoolean();
        }

        public virtual Address ToAddress()
        {
            return ToValue().ToAddress();
        }

        public virtual Bytes ToBytes()
        {
            return ToValue().ToBytes();
        }
        
        public override string ToString()
        {
            return ToValue().ToString();
        }
    }

    public class RpcValueException : ArgumentException
    {
        public RpcValueException(string message) : base(message)
        {
            
        }
    }
}
