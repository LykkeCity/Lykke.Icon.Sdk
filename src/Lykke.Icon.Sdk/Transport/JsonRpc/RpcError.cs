using System.Runtime.Serialization;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
    [DataContract]
    public class RpcError
    {
        public RpcError(long code, string message)
        {
            Code = code;
            Message = message;
        }

        [DataMember(Name = "code")]
        public long Code { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        public RpcErrorException ToException()
        {
            return new RpcErrorException(Code, Message);
        }
    }
}
