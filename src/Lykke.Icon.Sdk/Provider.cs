using System.Threading.Tasks;
using Lykke.Icon.Sdk.Transport.JsonRpc;

namespace Lykke.Icon.Sdk
{
    /**
     * Provider class transports the request and receives the response
     */
    public interface IProvider
    {
        /**
         * Prepares to execute the request
         *
         * @param request   A request to send
         * @param converter converter converter for the responseType
         * @param <T>       returning type
         * @return a Request object to execute
         */
        Task<T> Request<T>(Request request, RpcConverter<T> converter);
    }
}