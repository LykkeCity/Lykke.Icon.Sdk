using System.Numerics;
using Lykke.Icon.Sdk.Data;
using Lykke.Icon.Sdk.Transport.JsonRpc;

namespace Lykke.Icon.Sdk
{
    public class TransactionData
    {
        public RpcItem Data { get; set; }
        
        public string DataType { get; set; }

        public Address From { get; set; }
        
        public BigInteger? Nid = (long)NetworkId.Main;
        public BigInteger? Nonce { get; set; }
        public BigInteger? StepLimit { get; set; }
        public BigInteger? Timestamp { get; set; }

        public Address To { get; set; }

        public BigInteger? Value { get; set; }

        public BigInteger Version = BigInteger.Parse("3");

        
        public ITransaction Build()
        {
            TransactionBuilder.CheckAddress(From, "from not found");
            TransactionBuilder.CheckAddress(To, "to not found");
            TransactionBuilder.CheckArgument(Version, "version not found");
            TransactionBuilder.CheckArgument(StepLimit, "stepLimit not found");
            
            return new SendingTransaction(this);
        }
    }
}