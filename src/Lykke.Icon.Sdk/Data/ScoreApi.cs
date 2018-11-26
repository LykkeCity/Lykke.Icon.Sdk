using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Icon.Sdk.Crypto;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Data
{
    public class ScoreApi
    {
        private RpcObject properties;

        ScoreApi(RpcObject properties)
        {
            this.properties = properties;
        }

        public RpcObject getProperties()
        {
            return properties;
        }

        public String GetType()
        {
            RpcItem item = properties.GetItem("type");
            return item != null ? item.ToString() : null;
        }

        public String GetName()
        {
            RpcItem item = properties.GetItem("name");
            return item != null ? item.ToString() : null;
        }

        public List<Param> getInputs()
        {
            return GetParams(properties.GetItem("inputs"));
        }

        public List<Param> getOutputs()
        {
            return GetParams(properties.GetItem("outputs"));
        }

        List<Param> GetParams(RpcItem item)
        {
            List<Param> @params = new ArrayList<>();
            if (item != null)
            {
                foreach (RpcItem rpcItem in item.ToArray())
                {
                    RpcObject @object = (RpcObject)rpcItem;
                    @params.add(new Param(@object));
                }
            }

            return @params;
        }

        public String GetReadonly()
        {
            RpcItem item = properties.GetItem("readonly");
            return item != null ? item.ToString() : null;
        }

        public override String ToString()
        {
            return "ScoreApi{" +
                   "properties=" + properties +
                   '}';
        }

        public class Param
        {
            private RpcObject properties;

            Param(RpcObject properties)
            {
                this.properties = properties;
            }

            public String GetType()
            {
                RpcItem item = properties.GetItem("type");
                return item != null ? item.ToString() : null;
            }

            public String GetName()
            {
                RpcItem item = properties.GetItem("name");
                return item != null ? item.ToString() : null;
            }

            public BigInteger GetIndexed()
            {
                RpcItem item = properties.GetItem("indexed");
                return item != null ? item.asInteger() : null;
            }

            public String ToString()
            {
                return "Param{" +
                       "properties=" + properties +
                       '}';
            }
        }
    }
}

