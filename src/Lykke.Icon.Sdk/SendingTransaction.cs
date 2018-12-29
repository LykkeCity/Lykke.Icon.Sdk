using System.Numerics;
using Lykke.Icon.Sdk.Data;
using Lykke.Icon.Sdk.Transport.JsonRpc;

namespace Lykke.Icon.Sdk
{
    public class SendingTransaction : ITransaction
    {
        private readonly RpcItem _data;
        private readonly string _dataType;
        private readonly Address _from;
        private readonly BigInteger? _nid;
        private readonly BigInteger? _nonce;
        private readonly BigInteger? _stepLimit;
        private readonly BigInteger? _timestamp;
        private readonly Address _to;
        private readonly BigInteger? _value;
        private readonly BigInteger _version;

        
        public SendingTransaction(TransactionData transactionData)
        {
            _version = transactionData.Version;
            _from = transactionData.From;
            _to = transactionData.To;
            _value = transactionData.Value;
            _stepLimit = transactionData.StepLimit;
            _timestamp = transactionData.Timestamp;
            _nid = transactionData.Nid;
            _nonce = transactionData.Nonce;
            _dataType = transactionData.DataType;
            _data = transactionData.Data;
        }


        public RpcItem GetData()
            => _data;

        public string GetDataType()
            => _dataType;

        public Address GetFrom()
            => _from;

        public BigInteger? GetNid()
            => _nid;

        public BigInteger? GetNonce()
            => _nonce;

        public BigInteger? GetStepLimit()
            => _stepLimit;

        public BigInteger? GetTimestamp()
            => _timestamp;

        public Address GetTo()
            => _to;

        public BigInteger GetVersion()
            => _version;

        public BigInteger? GetValue()
            => _value;
    }
}