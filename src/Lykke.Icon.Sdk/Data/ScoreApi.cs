using System.Collections.Generic;
using System.Numerics;
using JetBrains.Annotations;
using Lykke.Icon.Sdk.Transport.JsonRpc;

namespace Lykke.Icon.Sdk.Data
{
    [PublicAPI]
    public class ScoreApi
    {
        private readonly RpcObject _properties;

        public ScoreApi(RpcObject properties)
        {
            _properties = properties;
        }

        public List<Param> GetScoreInputs()
        {
            return GetParams(_properties.GetItem("inputs"));
        }

        public string GetScoreName()
        {
            var item = _properties.GetItem("name");
            
            return item?.ToString();
        }

        public List<Param> GetScoreOutputs()
        {
            return GetParams(_properties.GetItem("outputs"));
        }

        public RpcObject GetScoreProperties()
        {
            return _properties;
        }

        public string GetScoreReadonly()
        {
            var item = _properties.GetItem("readonly");
            
            return item?.ToString();
        }

        public string GetScoreType()
        {
            var item = _properties.GetItem("type");
            
            return item?.ToString();
        }

        public override string ToString()
        {
            return "ScoreApi{properties=" + _properties + '}';
        }

        private static List<Param> GetParams(RpcItem item)
        {
            var @params = new List<Param>();
            
            if (item != null)
            {
                foreach (var rpcItem in item.ToArray())
                {
                    var @object = (RpcObject)rpcItem;
                    
                    @params.Add(new Param(@object));
                }
            }

            return @params;
        }

        public class Param
        {
            private readonly RpcObject _properties;

            public Param(RpcObject properties)
            {
                _properties = properties;
            }

            public BigInteger GetParamIndexed()
            {
                var item = _properties.GetItem("indexed");
                
                return item?.ToInteger() ?? 0;
            }

            public string GetParamName()
            {
                var item = _properties.GetItem("name");
                
                return item?.ToString();
            }

            public string GetParamType()
            {
                var item = _properties.GetItem("type");
                
                return item?.ToString();
            }

            public override string ToString()
            {
                return "Param{properties=" + _properties + '}';
            }
        }
    }
}

