using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Icon.Sdk.Crypto;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Data
{

/**
 * @see <a href="https://github.com/icon-project/icon-rpc-server/blob/develop/docs/icon-json-rpc-v3.md#icx_gettransactionresult" target="_blank">ICON JSON-RPC API</a>
 */
public class TransactionResult {

    private RpcObject properties;

    TransactionResult(RpcObject properties) {
        this.properties = properties;
    }

    public RpcObject GetProperties() {
        return properties;
    }

    /**
     * @return 1 on success, 0 on failure.
     */
    public BigInteger GetStatus() {
        RpcItem status = properties.GetItem("status");
        if (status != null) {
            return status.ToInteger();
        } else {
            // Migrates V2 block data
            // V2 Block data doesn't have a status field but have a code field
            // @see <a href="https://github.com/icon-project/icx_JSON_RPC#icx_gettransactionresult" target="_blank">ICON JSON-RPC V2 API</a>
            RpcItem code = properties.GetItem("code");
            if (code != null) return new BigInteger(code.ToInteger().IntValue() == 0 ? "1" : "0");
            else return null;
        }
    }

    /**
     * @return Recipient address of the transaction
     */
    public String getTo() {
        RpcItem item = properties.GetItem("to");
        return item != null ? item.ToString() : null;
    }

    /**
     * @return Transaction hash
     */
    public Bytes getTxHash() {
        RpcItem item = properties.GetItem("txHash");
        return item != null ? item.asBytes() : null;
    }

    /**
     * @return Transaction index in the block
     */
    public BigInteger getTxIndex() {
        RpcItem item = properties.GetItem("txIndex");
        return item != null ? item.ToInteger() : null;
    }

    /**
     * @return Height of the block that includes the transaction.
     */
    public BigInteger GetBlockHeight() {
        RpcItem item = properties.GetItem("blockHeight");
        return item != null ? item.ToInteger() : null;
    }

    /**
     * @return Hash of the block that includes the transation.
     */
    public Bytes GetBlockHash() {
        RpcItem item = properties.GetItem("blockHash");
        return item != null ? item.asBytes() : null;
    }

    /**
     * @return Sum of stepUsed by this transaction and all preceeding transactions in the same block.
     */
    public BigInteger GetCumulativeStepUsed() {
        RpcItem item = properties.GetItem("cumulativeStepUsed");
        return item != null ? item.ToInteger() : null;
    }

    /**
     * @return The amount of step used by this transaction.
     */
    public BigInteger GetStepUsed() {
        RpcItem item = properties.GetItem("stepUsed");
        return item != null ? item.ToInteger() : null;
    }

    /**
     * @return The step price used by this transaction.
     */
    public BigInteger GetStepPrice() {
        RpcItem item = properties.GetItem("stepPrice");
        return item != null ? item.ToInteger() : null;
    }

    /**
     * @return SCORE address if the transaction created a new SCORE.
     */
    public String GetScoreAddress() {
        RpcItem item = properties.GetItem("scoreAddress");
        return item != null ? item.ToString() : null;
    }

    /**
     * @return Bloom filter to quickly retrieve related eventlogs.
     */
    public String getLogsBloom() {
        RpcItem item = properties.GetItem("logsBloom");
        return item != null ? item.ToString() : null;
    }

    /**
     * @return List of event logs, which this transaction generated.
     */
    public List<EventLog> getEventLogs() {
        RpcItem item = properties.GetItem("eventLogs");
        List<EventLog> eventLogs = new ArrayList<>();
        if (item != null) {
            foreach (RpcItem rpcItem in item.asArray()) {
                eventLogs.add(new EventLog(rpcItem.asObject()));
            }
        }
        return eventLogs;
    }

    /**
     * @return This field exists when status is 0. Contains code(str) and message(str).
     */
    public Failure getFailure() {
        RpcItem failure = properties.GetItem("failure");

        if (failure == null) {
            BigInteger status = GetStatus();
            if (status != null && status.IntValue() == 0) {
                // Migrates V2 block data
                // V2 Block data doesn't have a failure field but have a code field
                // @see <a href="https://github.com/icon-project/icx_JSON_RPC#icx_gettransactionresult" target="_blank">ICON JSON-RPC V2 API</a>
                RpcItem code = properties.GetItem("code");
                if (code != null) {
                    RpcObject.Builder builder = new RpcObject.Builder();
                    builder.put("code", code);

                    RpcItem message = properties.GetItem("message");
                    if (message != null) {
                        builder.put("message", message);
                    }
                    failure = builder.build();
                }
            }
        }
        return failure != null ? new Failure(failure.asObject()) : null;
    }

    public override String ToString() {
        return "TransactionResult{" +
                "properties=" + properties +
                '}';
    }

    public class EventLog {
        private RpcObject properties;

        EventLog(RpcObject properties) {
            this.properties = properties;
        }

        public String GetScoreAddress() {
            RpcItem item = properties.GetItem("scoreAddress");
            return item != null ? item.ToString() : null;
        }

        public List<RpcItem> getIndexed() {
            RpcItem item = properties.GetItem("indexed");
            return item != null ? item.asArray().asList() : null;
        }

        public List<RpcItem> getData() {
            RpcItem field = properties.GetItem("data");
            return field != null ? field.asArray().asList() : null;
        }

        public override String ToString() {
            return "EventLog{" +
                    "properties=" + properties +
                    '}';
        }
    }

    public static class Failure {
        private RpcObject properties;

        private Failure(RpcObject properties) {
            this.properties = properties;
        }

        public BigInteger GetCode() {
            RpcItem item = properties.GetItem("code");
            return item != null ? item.ToInteger() : null;
        }

        public String GetMessage() {
            RpcItem item = properties.GetItem("message");
            return item != null ? item.ToString() : null;
        }

        public override String ToString() {
            return "Failure{" +
                    "properties=" + properties +
                    '}';
        }
        }
    }
}
