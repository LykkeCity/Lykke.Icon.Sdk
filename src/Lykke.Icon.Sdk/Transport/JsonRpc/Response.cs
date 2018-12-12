using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
    /**
     * A jsonrpc response of the request
     */
    [DataContract]
    public class Response
    {
        [DataMember(Name = "jsonrpc")]
        public String Jsonrpc { get; set; }

        [DataMember(Name = "method")]
        public String Method { get; set; }

        [DataMember(Name = "id")]
        public long Id { get; set; }

        [JsonConverter(typeof(RpcItemSerializer))]
        [DataMember(Name = "result")]
        public RpcItem Result { get; set; }

        [DataMember(Name = "error")]
        public RpcError Error { get; set; }
    }
}