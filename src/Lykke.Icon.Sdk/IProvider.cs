using System.Threading.Tasks;
using Lykke.Icon.Sdk.Transport.JsonRpc;

namespace Lykke.Icon.Sdk
{
    public interface IProvider
    {
        /// <summary>
        ///    Sends request
        /// </summary>
        Task<T> SendRequestAsync<T>(Request request, IRpcConverter<T> converter);
    }
}