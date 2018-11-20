using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Data
{
    /**
     * Provider class transports the request and receives the response
     */
    public interface Provider
    {

    /**
     * Prepares to execute the request
     *
     * @param request   A request to send
     * @param converter converter converter for the responseType
     * @param <O>       returning type
     * @return a Request object to execute
     */
    Request<O> Request(foundation.icon.icx.transport.jsonrpc.Request request, RpcConverter<O> converter);
    }
}