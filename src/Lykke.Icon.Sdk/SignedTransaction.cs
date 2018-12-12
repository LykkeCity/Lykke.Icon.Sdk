using Lykke.Icon.Sdk.Data;
using Lykke.Icon.Sdk.Transport.JsonRpc;
using Org.BouncyCastle.Crypto.Digests;
using System.Numerics;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lykke.Icon.Sdk
{
    /**
     * SignedTransaction Serializes transaction messages and
     * makes parameters to send
     */
    public class SignedTransaction
    {
        private Transaction _transaction;
        private Wallet _wallet;
        private RpcObject _properties;

        public SignedTransaction(Transaction transaction, Wallet wallet)
        {
            this._transaction = transaction;
            this._wallet = wallet;
            CreateProperties();
        }

        /**
         * Create the parameters including signature
         */
        private void CreateProperties()
        {
            RpcObject @object = GetTransactionProperties();

            RpcObject.Builder builder = new RpcObject.Builder();
            foreach (String key in @object.GetKeys().OrderBy(x => x))
            {
                builder.Put(key, @object.GetItem(key));
            }

            String signature = Base64.ToBase64String(GetSignature(@object));
            builder.Put("signature", new RpcValue(signature));
            this._properties = builder.Build();
        }

        /**
         * Gets the parameters including signature
         *
         * @return parameters
         */
        public RpcObject GetProperties()
        {
            return _properties;
        }

        public RpcObject GetTransactionProperties()
        {
            BigInteger? timestamp = _transaction.GetTimestamp();
            if (timestamp == null)
            {
                timestamp = BigInteger.Parse((DateTime.UtcNow.Millisecond * 1000L).ToString());
            }

            var builder = new RpcObject.Builder();
            PutTransactionPropertyToBuilder(builder, "version", _transaction.GetVersion());
            PutTransactionPropertyToBuilder(builder, "from", _transaction.GetFrom());
            PutTransactionPropertyToBuilder(builder, "to", _transaction.GetTo());
            PutTransactionPropertyToBuilder(builder, "value", _transaction.GetValue());
            PutTransactionPropertyToBuilder(builder, "stepLimit", _transaction.GetStepLimit());
            PutTransactionPropertyToBuilder(builder, "timestamp", timestamp);
            PutTransactionPropertyToBuilder(builder, "nid", _transaction.GetNid());
            PutTransactionPropertyToBuilder(builder, "nonce", _transaction.GetNonce());
            PutTransactionPropertyToBuilder(builder, "dataType", _transaction.GetDataType());
            PutTransactionPropertyToBuilder(builder, "data", _transaction.GetData());
            return builder.Build();
        }

        private void PutTransactionPropertyToBuilder(RpcObject.Builder builder, String key, BigInteger? value)
        {
            if (value.HasValue) builder.Put(key, new RpcValue(value.Value));
        }

        private void PutTransactionPropertyToBuilder(RpcObject.Builder builder, String key, String value)
        {
            if (value != null) builder.Put(key, new RpcValue(value));
        }

        private void PutTransactionPropertyToBuilder(RpcObject.Builder builder, String key, Address value)
        {
            if (value != null) builder.Put(key, new RpcValue(value));
        }

        private void PutTransactionPropertyToBuilder(RpcObject.Builder builder, String key, RpcItem item)
        {
            if (item != null) builder.Put(key, item);
        }

        /**
         * Gets the signature of the transaction
         *
         * @return signature
         */
        byte[] GetSignature(RpcObject properties)
        {
            return _wallet.Sign(Sha256(Serialize(properties)));
        }

        /**
         * Generates the hash of data
         *
         * @return hash
         */
        byte[] Sha256(String data)
        {
            byte[] hash = Encoding.UTF8.GetBytes(data);
            var digest = new Sha3Digest(256);
            var output = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(hash, 0, hash.Length);
            digest.DoFinal(output, 0);

            return output;
        }

        /**
         * Serializes the properties
         *
         * @return Serialized property
         */
        public String Serialize(RpcObject properties)
        {
            return TransactionSerializer.Serialize(properties);
        }

        /**
         * Transaction Serializer for generating a signature with transaction properties.
         */
        public static class TransactionSerializer
        {
            /**
             * Serializes properties as string
             *
             * @param properties transaction properties
             * @return Serialized string of properties
             */
            public static String Serialize(RpcObject properties)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("icx_sendTransaction.");
                SerializeObjectItems(builder, properties);
                return builder.ToString();
            }

            static void Serialize(StringBuilder builder, RpcItem item)
            {
                if (item is RpcObject)
                {
                    builder.Append("{");
                    SerializeObjectItems(builder, item.ToObject());
                    builder.Append("}");
                }
                else if (item is RpcArray)
                {
                    builder.Append("[");
                    SerializeArrayItems(builder, item.ToArray());
                    builder.Append("]");
                }
                else
                {
                    if (item == null)
                    {
                        builder.Append("\\0");
                    }
                    else
                    {
                        builder.Append(Escape(item.ToString()));
                    }
                }
            }

            private static void SerializeObjectItems(StringBuilder builder, RpcObject @object)
            {
                bool firstItem = true;
                // Sorts keys before serializing object
                var keys = @object.GetKeys().OrderBy(x => x);
                foreach (String key in keys)
                {
                    if (firstItem)
                    {
                        firstItem = false;
                    }
                    else
                    {
                        builder.Append(".");
                    }
                    Serialize(builder.Append(key).Append("."), @object.GetItem(key));
                }
            }

            private static void SerializeArrayItems(StringBuilder builder, RpcArray array)
            {
                bool firstItem = true;
                foreach (RpcItem child in array)
                {
                    if (firstItem)
                    {
                        firstItem = false;
                    }
                    else
                    {
                        builder.Append(".");
                    }
                    Serialize(builder, child);
                }
            }

            public static String Escape(String @string)
            {
                var regex = new Regex("([\\\\.{}\\[\\]])");
                //var result =  regex.Replace(@string, "\\\\$1");
                //var result = Regex.Replace(@string, "([\\\\.{}\\[\\]])", "\\\\$1");
                var result = Regex.Escape(@string);
                return result;
            }
        }
    }
}
