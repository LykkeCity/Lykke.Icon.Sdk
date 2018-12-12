using Lykke.Icon.Sdk.Transport.JsonRpc;
using System.Numerics;
using System;
using System.Collections.Generic;

namespace Lykke.Icon.Sdk.Data
{
    /**
     * @see <a href="https://github.com/icon-project/icon-rpc-server/blob/develop/docs/icon-json-rpc-v3.md#icx_gettransactionresult" target="_blank">ICON JSON-RPC API</a>
     */
    public class TransactionResult
    {

        private RpcObject properties;

        public TransactionResult(RpcObject properties)
        {
            this.properties = properties;
        }

        public RpcObject GetProperties()
        {
            return properties;
        }

        /**
         * @return 1 on success, 0 on failure.
         */
        public BigInteger GetStatus()
        {
            RpcItem status = properties.GetItem("status");
            if (status != null)
            {
                return status.ToInteger();
            }
            else
            {
                // Migrates V2 block data
                // V2 Block data doesn't have a status field but have a code field
                // @see <a href="https://github.com/icon-project/icx_JSON_RPC#icx_gettransactionresult" target="_blank">ICON JSON-RPC V2 API</a>
                RpcItem code = properties.GetItem("code");
                if (code != null) return BigInteger.Parse(code.ToInteger() == 0 ? "1" : "0");
                else return 0;
            }
        }

        /**
         * @return Recipient address of the transaction
         */
        public String GetTo()
        {
            RpcItem item = properties.GetItem("to");
            return item != null ? item.ToString() : null;
        }

        /**
         * @return Transaction hash
         */
        public Bytes GetTxHash()
        {
            RpcItem item = properties.GetItem("txHash");
            return item != null ? item.ToBytes() : null;
        }

        /**
         * @return Transaction index in the block
         */
        public BigInteger GetTxIndex()
        {
            RpcItem item = properties.GetItem("txIndex");
            return item != null ? item.ToInteger() : 0;
        }

        /**
         * @return Height of the block that includes the transaction.
         */
        public BigInteger GetBlockHeight()
        {
            RpcItem item = properties.GetItem("blockHeight");
            return item != null ? item.ToInteger() : 0;
        }

        /**
         * @return Hash of the block that includes the transation.
         */
        public Bytes GetBlockHash()
        {
            RpcItem item = properties.GetItem("blockHash");
            return item != null ? item.ToBytes() : null;
        }

        /**
         * @return Sum of stepUsed by this transaction and all preceeding transactions in the same block.
         */
        public BigInteger GetCumulativeStepUsed()
        {
            RpcItem item = properties.GetItem("cumulativeStepUsed");
            return item != null ? item.ToInteger() : 0;
        }

        /**
         * @return The amount of step used by this transaction.
         */
        public BigInteger GetStepUsed()
        {
            RpcItem item = properties.GetItem("stepUsed");
            return item != null ? item.ToInteger() : 0;
        }

        /**
         * @return The step price used by this transaction.
         */
        public BigInteger GetStepPrice()
        {
            RpcItem item = properties.GetItem("stepPrice");
            return item != null ? item.ToInteger() : 0;
        }

        /**
         * @return SCORE address if the transaction created a new SCORE.
         */
        public String GetScoreAddress()
        {
            RpcItem item = properties.GetItem("scoreAddress");
            return item != null ? item.ToString() : null;
        }

        /**
         * @return Bloom filter to quickly retrieve related eventlogs.
         */
        public String GetLogsBloom()
        {
            RpcItem item = properties.GetItem("logsBloom");
            return item != null ? item.ToString() : null;
        }

        /**
         * @return List of event logs, which this transaction generated.
         */
        public List<EventLog> GetEventLogs()
        {
            RpcItem item = properties.GetItem("eventLogs");
            List<EventLog> eventLogs = new List<EventLog>();
            if (item != null)
            {
                foreach (RpcItem rpcItem in item.ToArray())
                {
                    eventLogs.Add(new EventLog(rpcItem.ToObject()));
                }
            }
            return eventLogs;
        }

        /**
         * @return This field exists when status is 0. Contains code(str) and message(str).
         */
        public Failure GetFailure()
        {
            RpcItem failure = properties.GetItem("failure");

            if (failure == null)
            {
                BigInteger status = GetStatus();
                if (status != null && status == 0)
                {
                    // Migrates V2 block data
                    // V2 Block data doesn't have a failure field but have a code field
                    // @see <a href="https://github.com/icon-project/icx_JSON_RPC#icx_gettransactionresult" target="_blank">ICON JSON-RPC V2 API</a>
                    RpcItem code = properties.GetItem("code");
                    if (code != null)
                    {
                        RpcObject.Builder builder = new RpcObject.Builder();
                        builder.Put("code", code);

                        RpcItem message = properties.GetItem("message");
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

        public override String ToString()
        {
            return "TransactionResult{" +
                    "properties=" + properties +
                    '}';
        }

        public class EventLog
        {
            private RpcObject properties;

            public EventLog(RpcObject properties)
            {
                this.properties = properties;
            }

            public String GetScoreAddress()
            {
                RpcItem item = properties.GetItem("scoreAddress");
                return item != null ? item.ToString() : null;
            }

            public List<RpcItem> GetIndexed()
            {
                RpcItem item = properties.GetItem("indexed");
                return item != null ? item.ToArray().ToList() : null;
            }

            public List<RpcItem> GetData()
            {
                RpcItem field = properties.GetItem("data");
                return field != null ? field.ToArray().ToList() : null;
            }

            public override String ToString()
            {
                return "EventLog{" +
                        "properties=" + properties +
                        '}';
            }
        }

        public class Failure
        {
            private RpcObject properties;

            public Failure(RpcObject properties)
            {
                this.properties = properties;
            }

            public BigInteger GetCode()
            {
                RpcItem item = properties.GetItem("code");
                return item != null ? item.ToInteger() : 0;
            }

            public String GetMessage()
            {
                RpcItem item = properties.GetItem("message");
                return item != null ? item.ToString() : null;
            }

            public override String ToString()
            {
                return "Failure{" +
                        "properties=" + properties +
                        '}';
            }
        }
    }
}
