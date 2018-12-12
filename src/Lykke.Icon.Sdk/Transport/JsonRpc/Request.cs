using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

//// Request
//{
//"jsonrpc": "2.0",
//"method": "$STRING1",
//"id": $INT,
//"params": {
//"$KEY1": "$VALUE1",
//"$KEY2": {
//"method": "$STRING2",
//"params": {
//"$KEY3": "$VALUE3"
//}
//}
//}
//}

//// Response - success
//{
//"jsonrpc": "2.0",
//"id": $INT,
//"result": "$STRING"
//// or
//"result": {
//"$KEY1": "$VALUE1",
//"$KEY2": "$VALUE2"
//}
//}

//// Response - error
//{
//"jsonrpc": "2.0",
//"id": $INT1,
//"error": {
//"code": $INT2,
//"message": "$STRING"
//}
//}
namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
    /**
     * A jsonrpc request to be execute
     */
    [DataContract]
    public class Request
    {
        public Request(long id, String method, RpcObject @params)
        {
            this.Id = id;
            this.Method = method;
            this.Params = @params;
            Jsonrpc = "2.0";
        }

        [DataMember(Name = "jsonrpc")]
        public String Jsonrpc { get; protected set; }

        [DataMember(Name = "id")]
        public long Id { get; protected set; }

        [DataMember(Name = "method")]
        public String Method { get; protected set; }

        [DataMember(Name = "params")]
        [JsonConverter(typeof(RpcItemSerializer))]
        public RpcObject Params{ get; protected set; }
    }
}