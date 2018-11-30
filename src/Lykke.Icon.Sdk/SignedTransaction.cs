using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Lykke.Icon.Sdk.Data;
using Lykke.Icon.Sdk.Transport.JsonRpc;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk
{

    /**
     * SignedTransaction Serializes transaction messages and
     * makes parameters to send
     */
    public class SignedTransaction
    {

        private Transaction transaction;
        private Wallet wallet;
        private RpcObject properties;

        public SignedTransaction(Transaction transaction, Wallet wallet)
        {
            this.transaction = transaction;
            this.wallet = wallet;
            CreateProperties();
        }

        /**
         * Create the parameters including signature
         */
        private void CreateProperties()
        {
            RpcObject @object = GetTransactionProperties();

            RpcObject.Builder builder = new RpcObject.Builder();
            foreach (String key in @object.GetKeys())
            {
                builder.Put(key, @object.GetItem(key));
            }

            String signature = Base64.ToBase64String(GetSignature(@object));
            builder.Put("signature", new RpcValue(signature));
            this.properties = builder.Build();
        }

        /**
         * Gets the parameters including signature
         *
         * @return parameters
         */
        public RpcObject GetProperties()
        {
            return properties;
        }

        RpcObject GetTransactionProperties()
        {
            BigInteger timestamp = transaction.GetTimestamp();
            if (timestamp == null)
            {
                timestamp = new BigInteger((DateTime.UtcNow.Millisecond * 1000L).ToString());
            }

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

        private void PutTransactionPropertyToBuilder(RpcObject.Builder builder, String key, BigInteger value)
        {
            if (value != null) builder.Put(key, new RpcValue(value));
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
            return wallet.Sign(Sha256(Serialize(properties)));
        }

        /**
         * Generates the hash of data
         *
         * @return hash
         */
        byte[] Sha256(String data)
        {
            byte[] hash = Encoding.UTF8.GetBytes(data);
            new Sha3Digest(256).DoFinal(hash, 0);
            return hash;
        }

        /**
         * Serializes the properties
         *
         * @return Serialized property
         */
        String Serialize(RpcObject properties)
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
                var keys = (@object.GetKeys());
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

            static String Escape(String @string)
            {
                var result = Regex.Replace(@string, "([\\\\.{}\\[\\]])", "\\\\$1");
                return result;
            }
        }
    }
}
