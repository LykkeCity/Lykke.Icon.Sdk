using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
    /**
     * A jsonrpc response of the request
     */
    public class Response
    {
        private String jsonrpc = "2.0";

        private String method;

        private long id;

        private RpcItem result;

        private RpcError error;

        public String GetJsonrpc()
        {
            return jsonrpc;
        }

        public String GetMethod()
        {
            return method;
        }

        public long GetId()
        {
            return id;
        }

        public RpcItem GetResult()
        {
            return result;
        }

        public RpcError GetError()
        {
            return error;
        }
    }
}