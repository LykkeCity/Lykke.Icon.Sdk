using System;
using System.Linq;
using System.Numerics;
using System.Text;
using JetBrains.Annotations;
using Lykke.Icon.Sdk.Crypto;
using Lykke.Icon.Sdk.Data;
using Lykke.Icon.Sdk.Transport.JsonRpc;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk
{
    /// <inheritdoc />
    [PublicAPI]
    public class SignedTransaction : ITransaction
    {
        private readonly RpcObject _properties;
        private readonly ITransaction _transaction;

        protected SignedTransaction(ITransaction transaction, RpcObject properties)
        {
            _transaction = transaction;
            _properties = properties;
        }

        public SignedTransaction(ITransaction transaction, IWallet wallet)
        {
            _transaction = transaction;
            _properties = CreateProperties(transaction, wallet);
        }

        /// <inheritdoc />
        public RpcItem GetData()
        {
            return _transaction.GetData();
        }

        /// <inheritdoc />
        public string GetDataType()
        {
            return _transaction.GetDataType();
        }

        /// <inheritdoc />
        public Address GetFrom()
        {
            return _transaction.GetFrom();
        }

        /// <inheritdoc />
        public BigInteger? GetNid()
        {
            return _transaction.GetNid();
        }

        /// <inheritdoc />
        public BigInteger? GetNonce()
        {
            return _transaction.GetNonce();
        }

        /// <summary>
        ///    Get transaction parameters including signature 
        /// </summary>
        public RpcObject GetProperties()
        {
            return _properties;
        }
        
        /// <inheritdoc />
        public BigInteger? GetStepLimit()
        {
            return _transaction.GetStepLimit();
        }

        /// <inheritdoc />
        public BigInteger? GetTimestamp()
        {
            return _transaction.GetTimestamp();
        }

        /// <inheritdoc />
        public Address GetTo()
        {
            return _transaction.GetTo();
        }

        /// <summary>
        ///    Get transaction parameters 
        /// </summary>
        public RpcObject GetTransactionProperties()
        {
            return GetTransactionProperties(_transaction);
        }
        
        /// <inheritdoc />
        public BigInteger GetVersion()
        {
            return _transaction.GetVersion();
        }

        /// <inheritdoc />
        public BigInteger? GetValue()
        {
            return _transaction.GetValue();
        }
        
        /// <summary>
        ///     Serializes the properties 
        /// </summary>
        public static string Serialize(RpcObject properties)
        {
            return TransactionSerializer.Serialize(properties);
        }

        public static SignedTransaction Deserialize(string transactionSerialized)
        {
            var (transaction, rpcObject) = TransactionDeserializer.DeserializeToTransactionAndRpc(transactionSerialized);
            var signature = rpcObject.GetItem("signature");
            if (signature == null)
                throw new InvalidOperationException("Signature is not provided");

            var sign = Base64.Decode(signature.ToString());
            var sender = transaction.GetFrom();
            var props = GetTransactionProperties(transaction);
            var transactionHash = GetTransactionHash(props);
            if (!EcdsaSignature.VerifySignature(sender, sign, transactionHash))
                throw new ArgumentException("Signature does not match", nameof(transactionSerialized));

            var signedTransaction = new SignedTransaction(transaction, rpcObject);

            return signedTransaction;
        }

        private static RpcObject CreateProperties(ITransaction transaction, IWallet wallet)
        {
            var builder = new RpcObject.Builder();
            var @object = GetTransactionProperties(transaction);
            var signature = GetSignature(wallet, @object);
            
            foreach (var key in @object.GetKeys().OrderBy(x => x))
            {
                builder.Put(key, @object.GetItem(key));
            }

            builder.Put("signature", new RpcValue(signature));
            
            return builder.Build();
        }
        
        public static string GetSignature(IWallet wallet, RpcObject properties)
        {
            var transactionHash = GetTransactionHash(properties);
            var signature =  wallet.Sign(transactionHash);

            return Base64.ToBase64String(signature);
        }

        public static byte[] GetTransactionHash(RpcObject properties)
        {
            var serialized = Serialize(properties);
            var hash = Sha256(serialized);

            return hash;
        }

        public static RpcObject GetTransactionProperties(ITransaction transaction)
        {
            var timestamp = transaction.GetTimestamp() ?? BigInteger.Parse((DateTime.UtcNow.Millisecond * 1000L).ToString());

            var builder = new RpcObject.Builder();
            
            PutTransactionPropertyToBuilder(builder, "version", transaction.GetVersion());
            PutTransactionPropertyToBuilder(builder, "from", transaction.GetFrom());
            PutTransactionPropertyToBuilder(builder, "to", transaction.GetTo());
            PutTransactionPropertyToBuilder(builder, "value", transaction.GetValue());
            PutTransactionPropertyToBuilder(builder, "stepLimit", transaction.GetStepLimit());
            PutTransactionPropertyToBuilder(builder, "timestamp", timestamp);
            PutTransactionPropertyToBuilder(builder, "nid", transaction.GetNid());
            PutTransactionPropertyToBuilder(builder, "nonce", transaction.GetNonce());
            PutTransactionPropertyToBuilder(builder, "dataType", transaction.GetDataType());
            PutTransactionPropertyToBuilder(builder, "data", transaction.GetData());
            
            return builder.Build();
        }
        
        private static void PutTransactionPropertyToBuilder(RpcObject.Builder builder, string key, BigInteger? value)
        {
            if (value.HasValue) builder.Put(key, new RpcValue(value.Value));
        }

        private static void PutTransactionPropertyToBuilder(RpcObject.Builder builder, string key, string value)
        {
            if (value != null) builder.Put(key, new RpcValue(value));
        }

        private static void PutTransactionPropertyToBuilder(RpcObject.Builder builder, string key, Address value)
        {
            if (value != null) builder.Put(key, new RpcValue(value));
        }

        private static void PutTransactionPropertyToBuilder(RpcObject.Builder builder, string key, RpcItem item)
        {
            if (item != null) builder.Put(key, item);
        }
        
        private static byte[] Sha256(string data)
        {
            var input = Encoding.UTF8.GetBytes(data);
            var digest = new Sha3Digest(256);
            var output = new byte[digest.GetDigestSize()];
            
            digest.BlockUpdate(input, 0, input.Length);
            digest.DoFinal(output, 0);

            return output;
        }
    }
}
