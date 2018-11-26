using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk
{

    /**
     * SignedTransaction serializes transaction messages and
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
            foreach (String key in @object.KeySet())
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
                timestamp = new BigInteger(Long.ToString(System.CurrentTimeMillis() * 1000L));
            }

            Builder builder = new Builder();
            putTransactionPropertyToBuilder(builder, "version", transaction.GetVersion());
            putTransactionPropertyToBuilder(builder, "from", transaction.GetFrom());
            putTransactionPropertyToBuilder(builder, "to", transaction.GetTo());
            putTransactionPropertyToBuilder(builder, "value", transaction.GetValue());
            putTransactionPropertyToBuilder(builder, "stepLimit", transaction.GetStepLimit());
            putTransactionPropertyToBuilder(builder, "timestamp", timestamp);
            putTransactionPropertyToBuilder(builder, "nid", transaction.GetNid());
            putTransactionPropertyToBuilder(builder, "nonce", transaction.GetNonce());
            putTransactionPropertyToBuilder(builder, "dataType", transaction.GetDataType());
            putTransactionPropertyToBuilder(builder, "data", transaction.GetData());
            return builder.build();
        }

        private void putTransactionPropertyToBuilder(Builder builder, String key, BigInteger value)
        {
            if (value != null) builder.put(key, new RpcValue(value));
        }

        private void putTransactionPropertyToBuilder(Builder builder, String key, String value)
        {
            if (value != null) builder.put(key, new RpcValue(value));
        }

        private void putTransactionPropertyToBuilder(Builder builder, String key, Address value)
        {
            if (value != null) builder.put(key, new RpcValue(value));
        }

        private void putTransactionPropertyToBuilder(Builder builder, String key, RpcItem item)
        {
            if (item != null) builder.put(key, item);
        }

        /**
         * Gets the signature of the transaction
         *
         * @return signature
         */
        byte[] getSignature(RpcObject properties)
        {
            return wallet.sign(sha256(serialize(properties)));
        }

        /**
         * Generates the hash of data
         *
         * @return hash
         */
        byte[] sha256(String data)
        {
            return new SHA3.Digest256().digest(data.getBytes());
        }

        /**
         * Serializes the properties
         *
         * @return Serialized property
         */
        String serialize(RpcObject properties)
        {
            return TransactionSerializer.serialize(properties);
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
             * @return serialized string of properties
             */
            public static String serialize(RpcObject properties)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("icx_sendTransaction.");
                serializeObjectItems(builder, properties);
                return builder.ToString();
            }

            static void serialize(StringBuilder builder, RpcItem item)
            {
                if (item is RpcObject)
                {
                    builder.Append("{");
                    serializeObjectItems(builder, item.asObject());
                    builder.Append("}");
                }
                else if (item is RpcArray)
                {
                    builder.Append("[");
                    serializeArrayItems(builder, item.asArray());
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
                        builder.Append(escape(item.asString()));
                    }
                }
            }

            private static void serializeObjectItems(StringBuilder builder, RpcObject @object)
            {
                boolean firstItem = true;
                // Sorts keys before serializing object
                TreeSet<String> keys = new TreeSet<>(@object.keySet());
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
                    serialize(builder.Append(key).Append("."), object.getItem(key));
                }
            }

            private static void serializeArrayItems(StringBuilder builder, RpcArray array)
            {
                boolean firstItem = true;
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
                    serialize(builder, child);
                }
            }

            static String escape(String @string)
            {
                return @string.replaceAll("([\\\\.{}\\[\\]])", "\\\\$1");
            }
        }
    }
}
