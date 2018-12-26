using System.Runtime.Serialization;
using Newtonsoft.Json;

// Request
// {
//   "jsonrpc": "2.0",
//   "method": "$STRING1",
//   "id": $INT,
//   "params": {
//     "$KEY1": "$VALUE1",
//     "$KEY2": {
//       "method": "$STRING2",
//       "params": {
//         "$KEY3": "$VALUE3"
//       }
//     }
//   }
// }

// Response - success
// {
//   "jsonrpc": "2.0",
//   "id": $INT,
//   "result": "$STRING"
//   // or
//   "result": {
//     "$KEY1": "$VALUE1",
//     "$KEY2": "$VALUE2"
//   }
// }

// Response - error
// {
//   "jsonrpc": "2.0",
//   "id": $INT1,
//   "error": {
//     "code": $INT2,
//     "message": "$STRING"
//   }
// }

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
    [DataContract]
    public class Request
    {
        public Request(long id, string method, RpcObject @params)
        {
            Id = id;
            Method = method;
            Params = @params;
            Jsonrpc = "2.0";
        }

        [DataMember(Name = "jsonrpc")]
        public string Jsonrpc { get; protected set; }

        [DataMember(Name = "id")]
        public long Id { get; protected set; }

        [DataMember(Name = "method")]
        public string Method { get; protected set; }

        [DataMember(Name = "params", EmitDefaultValue = false)]
        [JsonConverter(typeof(RpcItemSerializer))]
        public RpcObject Params{ get; protected set; }
    }
}