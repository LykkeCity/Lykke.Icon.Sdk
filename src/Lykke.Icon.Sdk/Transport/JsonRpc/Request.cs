using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
    /**
     * A jsonrpc request to be execute
     */
    public class Request
    {

        private String jsonrpc = "2.0";

        private long id;

        private String method;

        private RpcObject @params;

    public Request(long id, String method, RpcObject @params)
        {
            this.id = id;
            this.method = method;
            this.@params = @params;
        }

        public String GetJsonrpc()
        {
            return jsonrpc;
        }

        public long GetId()
        {
            return id;
        }

        public String GetMethod()
        {
            return method;
        }

        public RpcObject GetParams()
        {
            return @params;
        }
    }
}