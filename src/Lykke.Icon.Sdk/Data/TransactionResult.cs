using System.Collections.Generic;
using System.Numerics;
using JetBrains.Annotations;
using Lykke.Icon.Sdk.Transport.JsonRpc;

namespace Lykke.Icon.Sdk.Data
{
    [PublicAPI]
    public class TransactionResult
    {
        private readonly RpcObject _properties;

        public TransactionResult(RpcObject properties)
        {
            _properties = properties;
        }

        public RpcObject GetProperties()
        {
            return _properties;
        }

        public BigInteger GetStatus()
        {
            var status = _properties.GetItem("status");
            
            if (status != null)
            {
                return status.ToInteger();
            }
            else
            {
                // Migrates V2 block data
                // V2 Block data doesn't have a status field but have a code field
                
                var code = _properties.GetItem("code");

                return code != null ? BigInteger.Parse(code.ToInteger() == 0 ? "1" : "0") : 0;
            }
        }
        
        public string GetTo()
        {
            var item = _properties.GetItem("to");
            
            return item?.ToString();
        }
        
        public Bytes GetTxHash()
        {
            var item = _properties.GetItem("txHash");
            
            return item?.ToBytes();
        }
        
        public BigInteger GetTxIndex()
        {
            var item = _properties.GetItem("txIndex");
            
            return item?.ToInteger() ?? 0;
        }
        
        public BigInteger GetBlockHeight()
        {
            var item = _properties.GetItem("blockHeight");
            
            return item?.ToInteger() ?? 0;
        }

        public Bytes GetBlockHash()
        {
            var item = _properties.GetItem("blockHash");
            
            return item?.ToBytes();
        }

        public BigInteger GetCumulativeStepUsed()
        {
            var item = _properties.GetItem("cumulativeStepUsed");
            
            return item?.ToInteger() ?? 0;
        }
        
        public BigInteger GetStepUsed()
        {
            var item = _properties.GetItem("stepUsed");
            
            return item?.ToInteger() ?? 0;
        }

        public BigInteger GetStepPrice()
        {
            var item = _properties.GetItem("stepPrice");
            
            return item?.ToInteger() ?? 0;
        }

        public string GetScoreAddress()
        {
            var item = _properties.GetItem("scoreAddress");
            
            return item?.ToString();
        }

        public string GetLogsBloom()
        {
            var item = _properties.GetItem("logsBloom");
            
            return item?.ToString();
        }

        public List<EventLog> GetEventLogs()
        {
            var item = _properties.GetItem("eventLogs");
            var eventLogs = new List<EventLog>();
            
            if (item != null)
            {
                foreach (var rpcItem in item.ToArray())
                {
                    eventLogs.Add(new EventLog(rpcItem.ToObject()));
                }
            }
            
            return eventLogs;
        }

        /// <summary>
        ///    This field exists when status is 0. Contains code(str) and message(str) 
        /// </summary>
        public Failure GetFailure()
        {
            var failure = _properties.GetItem("failure");

            if (failure == null)
            {
                var status = GetStatus();
                
                if (status == 0)
                {
                    // Migrates V2 block data
                    // V2 Block data doesn't have a failure field but have a code field
                    
                    var code = _properties.GetItem("code");
                    
                    if (code != null)
                    {
                        var builder = new RpcObject.Builder();
                        
                        builder.Put("code", code);

                        var message = _properties.GetItem("message");
                        if (message != null)
                        {
                            builder.Put("message", message);
                        }

                        failure = builder.Build();
                    }
                }
            }
            
            return failure != null ? new Failure(failure.ToObject()) : null;
        }

        public override string ToString()
        {
            return "TransactionResult{properties=" + _properties + '}';
        }

        [PublicAPI]
        public class EventLog
        {
            private readonly RpcObject _properties;

            public EventLog(RpcObject properties)
            {
                _properties = properties;
            }

            public List<RpcItem> GetData()
            {
                var field = _properties.GetItem("data");
                
                return field?.ToArray().ToList();
            }

            public List<RpcItem> GetIndexed()
            {
                var item = _properties.GetItem("indexed");
                
                return item?.ToArray().ToList();
            }

            public string GetScoreAddress()
            {
                var item = _properties.GetItem("scoreAddress");
                
                return item?.ToString();
            }

            public override string ToString()
            {
                return "EventLog{properties=" + _properties + '}';
            }
        }

        [PublicAPI]
        public class Failure
        {
            private readonly RpcObject _properties;

            public Failure(RpcObject properties)
            {
                _properties = properties;
            }

            public BigInteger GetCode()
            {
                var item = _properties.GetItem("code");
                
                return item?.ToInteger() ?? 0;
            }

            public string GetMessage()
            {
                var item = _properties.GetItem("message");
                
                return item?.ToString();
            }

            public override string ToString()
            {
                return "Failure{properties=" + _properties + '}';
            }
        }
    }
}
