using Lykke.Icon.Sdk.Data;
using Lykke.Icon.Sdk.Transport.JsonRpc;
using Org.BouncyCastle.Crypto.Digests;
using System.Numerics;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
        private ITransaction _transaction;
        private IWallet _wallet;
        private RpcObject _properties;

        public SignedTransaction(ITransaction transaction, IWallet wallet)
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
        private byte[] GetSignature(RpcObject properties)
        {
            return _wallet.Sign(Sha256(Serialize(properties)));
        }

        /**
         * Generates the hash of data
         *
         * @return hash
         */
        private byte[] Sha256(String data)
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

        /// <summary>
        /// Can't deserialize arrays params
        /// </summary>
        public static class TransactionDeserializer
        {
            public static RpcObject DeserializeToRpc(string serializedObject)
            {
                var rpcBuilder = new RpcObject.Builder();
                var i = 0;
                while (serializedObject.Length > i)
                {
                    var key = ReadKey(ref i, serializedObject);
                    var value = ReadValue(ref i, serializedObject);
                    if (value != null)
                        rpcBuilder.Put(key, value);
                }

                return rpcBuilder.Build();
            }

            private static string ReadKey(ref int from, string serializedObject)
            {
                var sb = new StringBuilder(10);
                while (from < serializedObject.Length && serializedObject[from] != '.')
                {
                    sb.Append(serializedObject[from]);
                    from++;
                }

                from++;

                if (sb.Length == 0 && from < serializedObject.Length)
                    return ReadKey(ref from, serializedObject);

                return sb.ToString();
            }

            private static RpcItem ReadValue(ref int from, string serializedObject)
            {
                bool keepReading = true;
                var sb = new StringBuilder(10);
                while (from < serializedObject.Length)
                {
                    var character = serializedObject[from];
                    if (character == '{')
                    {
                        Stack<char> symbolStack = new Stack<char>();
                        symbolStack.Push(character);

                        while (symbolStack.Count != 0)
                        {
                            from++;
                            character = serializedObject[from];

                            if (character == '{')
                            {
                                symbolStack.Push(character);
                            }
                            else if (character == '}')
                            {
                                symbolStack.Pop();
                            }

                            sb.Append(character);
                        }

                        sb.Remove(sb.Length - 1, 1);
                        string objectSerialized = sb.ToString();
                        var @object = DeserializeToRpc(objectSerialized);
                        from++;

                        return @object;
                    }
                    //TODO: Process arrays if possible
                    //else if (character == '[')
                    //{

                    //}
                    while (from < serializedObject.Length && serializedObject[from] != '.')
                    {
                        sb.Append(serializedObject[from]);
                        from++;
                    }
                    from++;

                    return new RpcValue(sb.ToString());
                }

                return null;
            }

            public static ITransaction Deserialize(string serializedObject)
            {
                var keyDictionary = new Dictionary<string, object>();
                var sb = new StringBuilder();
                var transactionMarker = serializedObject.Substring(0, "icx_sendTransaction.".Length);
                if (transactionMarker != "icx_sendTransaction.")
                {
                    throw new ArgumentException("Transaction should start with icx_sendTransaction marker.");
                }

                var withoutMarker =
                    serializedObject.Substring("icx_sendTransaction.".Length);
                var deserilaizedRpcObject = DeserializeToRpc(withoutMarker);
                var transactionData = ConstructTransactionData(deserilaizedRpcObject);

                return transactionData.Build();
            }

            private static TransactionBuilder.TransactionData ConstructTransactionData(RpcObject @object)
            {
                var transactionData = new TransactionBuilder.TransactionData();

                transactionData.Data = @object.GetItem("data");

                transactionData.DataType = @object.GetItem("dataType")?.ToString();

                transactionData.From = new Address(@object.GetItem("from").ToString());

                transactionData.To = new Address(@object.GetItem("to").ToString());

                transactionData.Nid = @object.GetItem("nid")?.ToInteger();

                transactionData.Nonce = @object.GetItem("nonce")?.ToInteger();

                transactionData.StepLimit = @object.GetItem("stepLimit")?.ToInteger();

                transactionData.Timestamp = @object.GetItem("timestamp")?.ToInteger();

                transactionData.Value = @object.GetItem("value")?.ToInteger();

                transactionData.Version = @object.GetItem("version").ToInteger();

                return transactionData;
            }
        }
    }
}
