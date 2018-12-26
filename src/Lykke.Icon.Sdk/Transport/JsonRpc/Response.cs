using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
    [DataContract]
    public class Response
    {
        [DataMember(Name = "jsonrpc")]
        public string Jsonrpc { get; set; }

        [DataMember(Name = "method")]
        public string Method { get; set; }

        [DataMember(Name = "id")]
        public long Id { get; set; }

        [JsonConverter(typeof(RpcItemSerializer))]
        [DataMember(Name = "result")]
        public RpcItem Result { get; set; }

        [DataMember(Name = "error")]
        public RpcError Error { get; set; }
    }
}