using Lykke.Icon.Sdk.Transport.JsonRpc;
using System.Numerics;
using System;
using System.Collections.Generic;

namespace Lykke.Icon.Sdk.Data
{
    public class ScoreApi
    {
        private RpcObject _properties;

        public ScoreApi(RpcObject properties)
        {
            this._properties = properties;
        }

        public RpcObject GetProperties()
        {
            return _properties;
        }

        public String GetType()
        {
            RpcItem item = _properties.GetItem("type");
            return item != null ? item.ToString() : null;
        }

        public String GetName()
        {
            RpcItem item = _properties.GetItem("name");
            return item != null ? item.ToString() : null;
        }

        public List<Param> getInputs()
        {
            return GetParams(_properties.GetItem("inputs"));
        }

        public List<Param> getOutputs()
        {
            return GetParams(_properties.GetItem("outputs"));
        }

        List<Param> GetParams(RpcItem item)
        {
            List<Param> @params = new List<Param>();
            if (item != null)
            {
                foreach (RpcItem rpcItem in item.ToArray())
                {
                    RpcObject @object = (RpcObject)rpcItem;
                    @params.Add(new Param(@object));
                }
            }

            return @params;
        }

        public String GetReadonly()
        {
            RpcItem item = _properties.GetItem("readonly");
            return item != null ? item.ToString() : null;
        }

        public override String ToString()
        {
            return "ScoreApi{" +
                   "properties=" + _properties +
                   '}';
        }

        public class Param
        {
            private RpcObject _properties;

            public Param(RpcObject properties)
            {
                this._properties = properties;
            }

            public String GetType()
            {
                RpcItem item = _properties.GetItem("type");
                return item != null ? item.ToString() : null;
            }

            public String GetName()
            {
                RpcItem item = _properties.GetItem("name");
                return item != null ? item.ToString() : null;
            }

            public BigInteger GetIndexed()
            {
                RpcItem item = _properties.GetItem("indexed");
                return item != null ? item.ToInteger() : 0;
            }

            public String ToString()
            {
                return "Param{" +
                       "properties=" + _properties +
                       '}';
            }
        }
    }
}

