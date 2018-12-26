using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Lykke.Icon.Sdk.Transport.JsonRpc;
using Newtonsoft.Json;

namespace Lykke.Icon.Sdk.Transport.Http
{
    public class HttpProvider : IProvider
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;
        private readonly RpcItemSerializer _rpcItemSerializer;

        public HttpProvider(HttpClient httpClient, string url)
        {
            _rpcItemSerializer = new RpcItemSerializer();
            _httpClient = httpClient;
            _url = url;
        }

        public async Task<T> SendRequestAsync<T>(Request request, IRpcConverter<T> converter)
        {
            var serializedRequest = JsonConvert.SerializeObject(request, _rpcItemSerializer);
            var content = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
            var httpResponse = await _httpClient.PostAsync(_url, content);
            var responseSerialized = await httpResponse.Content.ReadAsStringAsync();
            var response = (Response) JsonConvert.DeserializeObject(responseSerialized, typeof(Response), _rpcItemSerializer);

            if (response.Error != null)
            {
                var exception = response.Error.ToException();
                
                throw exception;
            }

            var convertedResult = converter.ConvertTo(response.Result);

            return convertedResult;
        }
    }
}