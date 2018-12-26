using System;
using System.Numerics;
using System.Text;
using JetBrains.Annotations;
using Lykke.Icon.Sdk.Data;
using Lykke.Icon.Sdk.Transport.JsonRpc;

namespace Lykke.Icon.Sdk
{
    [PublicAPI]
    public static class TransactionBuilder
    {
        public static void CheckAddress(Address address, string message)
        {
            CheckArgument(address, message);
            
            if (address.IsMalformed())
            {
                throw new ArgumentException("Invalid address");
            }
        }
        
        public static void CheckArgument<T>(T @object, string message)
        {
            if (@object == null)
            {
                throw new ArgumentException(message);
            }
        }
        
        /// <summary>
        ///    Creates a builder to create a transaction to send 
        /// </summary>
        public static Builder CreateBuilder()
        {
            return new Builder();
        }
        
        /// <summary>
        ///    Creates a builder for the given network ID 
        /// </summary>
        public static Builder Of(NetworkId nid)
        {
            var builder = CreateBuilder();
            
            return builder.Nid(nid);
        }

        /// <summary>
        ///    Creates a builder for the given network ID 
        /// </summary>
        public static Builder Of(BigInteger nid)
        {
            var builder = CreateBuilder();
            
            return builder.Nid(nid);
        }

        /// <summary>
        ///    A Builder for the simple icx sending transaction. 
        /// </summary>
        [PublicAPI]
        public class Builder
        {
            private readonly TransactionData _transactionData;

            internal Builder()
            {
                _transactionData = new TransactionData();
            }

            /// <summary>
            ///    Sets the Network ID
            /// </summary>
            public Builder Nid(BigInteger nid)
            {
                _transactionData.Nid = nid;
                
                return this;
            }

            /// <summary>
            ///    Sets the Network ID
            /// </summary>
            public Builder Nid(NetworkId nid)
            {
                _transactionData.Nid = (long)nid;
                
                return this;
            }

            /// <summary>
            ///    Sets the sender address
            /// </summary>
            public Builder From(Address from)
            {
                _transactionData.From = from;
                
                return this;
            }

            /// <summary>
            ///    Sets the receiver address
            /// </summary>
            public Builder To(Address to)
            {
                _transactionData.To = to;
                
                return this;
            }

            /// <summary>
            ///    Sets the value to send ICXs
            /// </summary>
            public Builder Value(BigInteger value)
            {
                _transactionData.Value = value;
                
                return this;
            }

            /// <summary>
            ///    Sets the maximum step
            /// </summary>
            public Builder StepLimit(BigInteger stepLimit)
            {
                _transactionData.StepLimit = stepLimit;
                
                return this;
            }

            /// <summary>
            ///    Sets the timestamp
            /// </summary>
            public Builder Timestamp(BigInteger timestamp)
            {
                _transactionData.Timestamp = timestamp;
                
                return this;
            }

            /// <summary>
            ///    Sets the nonce
            /// </summary>
            public Builder Nonce(BigInteger nonce)
            {
                _transactionData.Nonce = nonce;
                
                return this;
            }

            /// <summary>
            ///    Converts the builder to CallBuilder with the calling method name
            /// </summary>
            public CallBuilder Call(string method)
            {
                return new CallBuilder(_transactionData, method);
            }

            /// <summary>
            ///    Converts the builder to DeployBuilder with the deploying content
            /// </summary>
            public DeployBuilder Deploy(string contentType, byte[] content)
            {
                return new DeployBuilder(_transactionData, contentType, content);
            }

            /// <summary>
            ///    Converts the builder to MessageBuilder with the message
            /// </summary>
            public MessageBuilder Message(string message)
            {
                return new MessageBuilder(_transactionData, message);
            }

            /// <summary>
            ///    Makes a new transaction using given properties
            /// </summary>
            public ITransaction Build()
            {
                return _transactionData.Build();
            }

        }

        /// <summary>
        ///    A Builder for the calling SCORE transaction.
        /// </summary>
        [PublicAPI]
        public class CallBuilder
        {
            private readonly TransactionData _transactionData;
            private readonly RpcObject.Builder _dataBuilder;

            public CallBuilder(TransactionData transactionData, string method)
            {
                _transactionData = transactionData;
                _transactionData.DataType = "call";

                _dataBuilder = new RpcObject.Builder()
                        .Put("method", new RpcValue(method));
            }

            /// <summary>
            ///    Sets params 
            /// </summary>
            public CallBuilder Params(RpcObject @params)
            {
                _dataBuilder.Put("params", @params);
                
                return this;
            }

            /// <summary>
            ///    Sets params 
            /// </summary>
            public CallBuilder Params<T>(T @params)
            {
                _dataBuilder.Put("params", RpcItemCreator.Create(@params));
                
                return this;
            }

            /// <summary>
            ///    Creates a new transaction using given properties 
            /// </summary>
            public ITransaction Build()
            {
                _transactionData.Data = _dataBuilder.Build();
                
                CheckArgument(((RpcObject)_transactionData.Data).GetItem("method"), "method not found");

                return _transactionData.Build();
            }
        }

        /// <summary>
        ///    A Builder for the message transaction
        /// </summary>
        [PublicAPI]
        public class MessageBuilder
        {
            private readonly TransactionData _transactionData;

            public MessageBuilder(TransactionData transactionData, string message)
            {
                _transactionData = transactionData;
                _transactionData.DataType = "message";
                _transactionData.Data = new RpcValue(Encoding.UTF8.GetBytes(message));
            }

            /// <summary>
            ///    Creates a new transaction using given properties 
            /// </summary>
            public ITransaction Build()
            {
                return _transactionData.Build();
            }

        }

        /// <summary>
        ///    A Builder for the deploy transaction
        /// </summary>
        [PublicAPI]
        public class DeployBuilder
        {
            private readonly TransactionData _transactionData;
            private readonly RpcObject.Builder _dataBuilder;

            public DeployBuilder(TransactionData transactionData, string contentType, byte[] content)
            {
                _transactionData = transactionData;
                _transactionData.DataType = "deploy";

                _dataBuilder = new RpcObject.Builder()
                    .Put("contentType", new RpcValue(contentType))
                    .Put("content", new RpcValue(content));
            }

            /// <summary>
            ///    Sets params 
            /// </summary>
            public DeployBuilder Params(RpcObject @params)
            {
                _dataBuilder.Put("params", @params);
                return this;
            }

            /// <summary>
            ///    Creates a new transaction using given properties 
            /// </summary>
            public ITransaction Build()
            {
                _transactionData.Data = _dataBuilder.Build();
                
                CheckArgument(((RpcObject)_transactionData.Data).GetItem("contentType"), "contentType not found");
                CheckArgument(((RpcObject)_transactionData.Data).GetItem("content"), "content not found");

                return _transactionData.Build();
            }
        }
    }
}
