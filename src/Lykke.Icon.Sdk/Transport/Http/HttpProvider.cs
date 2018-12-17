using Lykke.Icon.Sdk.Data;
using Lykke.Icon.Sdk.Transport.JsonRpc;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Lykke.Icon.Sdk.Transport.Http
{
    /**
     * HttpProvider class transports as http jsonrpc
     */
    public class HttpProvider : IProvider
    {
        private readonly HttpClient _httpClient;
        private readonly String _url;
        private readonly RpcItemSerializer _rpcItemSerializator;

        public HttpProvider(HttpClient httpClient, String url)
        {
            _rpcItemSerializator = new RpcItemSerializer();
            this._httpClient = httpClient;
            this._url = url;
        }

        public async Task<T> Request<T>(Request request, RpcConverter<T> converter)
        {
            var serializedRequest = JsonConvert.SerializeObject(request, _rpcItemSerializator);
            StringContent content = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
            var httpResponse = await _httpClient.PostAsync(_url, content);
            var responseSerialized = await httpResponse.Content.ReadAsStringAsync();
            var response = (Response) JsonConvert.DeserializeObject(responseSerialized, typeof(Response), _rpcItemSerializator);

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