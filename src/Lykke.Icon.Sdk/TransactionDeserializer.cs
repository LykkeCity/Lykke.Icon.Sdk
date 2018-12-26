using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Lykke.Icon.Sdk.Data;
using Lykke.Icon.Sdk.Transport.JsonRpc;

namespace Lykke.Icon.Sdk
{
    /// <remarks>
    ///    Can't deserialize arrays params
    /// </remarks>
    [PublicAPI]
    public static class TransactionDeserializer
    {
        public static ITransaction Deserialize(string serializedObject)
        {
            var transactionMarker = serializedObject.Substring(0, "icx_sendTransaction.".Length);
            
            if (transactionMarker != "icx_sendTransaction.")
            {
                throw new ArgumentException("Transaction should start with icx_sendTransaction marker.");
            }

            var withoutMarker = serializedObject.Substring("icx_sendTransaction.".Length);
            var rpcObject = DeserializeToRpc(withoutMarker);
            var transactionData = ConstructTransactionData(rpcObject);

            return transactionData.Build();
        }
        
        private static RpcObject DeserializeToRpc(string serializedObject)
        {
            var rpcBuilder = new RpcObject.Builder();
            var i = 0;
            
            while (serializedObject.Length > i)
            {
                var key = ReadKey(ref i, serializedObject);
                var value = ReadValue(ref i, serializedObject);
                
                if (value != null)
                {
                    rpcBuilder.Put(key, value);
                }
            }

            return rpcBuilder.Build();
        }

        private static string ReadKey(ref int from, string serializedObject)
        {
            while (true)
            {
                var sb = new StringBuilder(10);
                
                while (from < serializedObject.Length && serializedObject[from] != '.')
                {
                    sb.Append(serializedObject[from]);
                    from++;
                }

                from++;

                if (sb.Length == 0 && from < serializedObject.Length)
                {
                    continue;
                }

                return sb.ToString();
            }
        }

        private static RpcItem ReadValue(ref int from, string serializedObject)
        {
            var sb = new StringBuilder(10);
            
            while (from < serializedObject.Length)
            {
                var character = serializedObject[from];
                if (character == '{')
                {
                    var symbolStack = new Stack<char>();
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
                    var objectSerialized = sb.ToString();
                    var @object = DeserializeToRpc(objectSerialized);
                    from++;

                    return @object;
                }
                
                // TODO: Process arrays if possible
                
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

        private static TransactionData ConstructTransactionData(RpcObject @object)
        {
            return new TransactionData
            {
                Data = @object.GetItem("data"),
                DataType = @object.GetItem("dataType")?.ToString(),
                From = new Address(@object.GetItem("from").ToString()),
                To = new Address(@object.GetItem("to").ToString()),
                Nid = @object.GetItem("nid")?.ToInteger(),
                Nonce = @object.GetItem("nonce")?.ToInteger(),
                StepLimit = @object.GetItem("stepLimit")?.ToInteger(),
                Timestamp = @object.GetItem("timestamp")?.ToInteger(),
                Value = @object.GetItem("value")?.ToInteger(),
                Version = @object.GetItem("version").ToInteger()
            };
        }
    }
}