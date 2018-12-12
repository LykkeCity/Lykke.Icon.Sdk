using System;
using System.Runtime.Serialization;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
    public class RpcErrorException : Exception
    {
        public RpcErrorException()
        {
            // jackson needs a default constructor
        }

        public RpcErrorException(long code, String message) : base(message)
        {
            this.Code = code;
        }

        /**
         * Returns the code of rpc error
         * @return error code
         */
        public long Code { get; private set; }
    }

    /**
     * RpcError defines the error that occurred during communicating through jsonrpc
     */
    [DataContract]
    public class RpcError
    {
        public RpcError(long code, String message)
        {
            this.Code = code;
            this.Message = message;
        }

        /**
         * Returns the code of rpc error
         * @return error code
         */
        [DataMember(Name = "code")]
        public long Code { get; set; }

        /**
         * Returns the message of rpc error
         * @return error message
         */
        [DataMember(Name = "message")]
        public String Message { get; set; }

        public RpcErrorException ToException()
        {
            return new RpcErrorException(Code, Message);
        }
    }
}
