using Lykke.Icon.Sdk.Data;
using Lykke.Icon.Sdk.Transport.JsonRpc;
using System.Numerics;
using System;
using System.Text;

namespace Lykke.Icon.Sdk
{
    /**
     * Builder for the transaction to send<br>
     * There are four builder types.<br>
     * Builder is a basic builder to send ICXs.<br>
     * CallBuilder, DeployBuilder, MessageBuilder is an extended builder for each purpose.
     * They can be initiated from Builder.
     *
     * @see <a href="https://github.com/icon-project/icon-rpc-server/blob/develop/docs/icon-json-rpc-v3.md#icx_sendtransaction" target="_blank">ICON JSON-RPC API</a>
     */
    public class TransactionBuilder
    {
        /**
         * Creates a builder for the given network ID
         *
         * @param Nid network ID
         * @return new builder
         * @deprecated This method can be replaced by {@link #NewBuilder()}
         */
        public static Builder Of(NetworkId Nid)
        {
            Builder builder = NewBuilder();
            return builder.Nid(Nid);
        }

        /**
         * Creates a builder for the given network ID
         *
         * @param Nid network ID in BigInteger
         * @return new builder
         * @deprecated This method can be replaced by {@link #NewBuilder()}
         */
        public static Builder Of(BigInteger Nid)
        {
            Builder builder = NewBuilder();
            return builder.Nid(Nid);
        }

        /**
         * Creates a builder to make a transaction to send
         *
         * @return new builder
         */
        public static Builder NewBuilder()
        {
            return new Builder();
        }

        /**
         * A Builder for the simple icx sending transaction.
         */
        public class Builder
        {
            private TransactionData transactionData;

            public Builder()
            {
                this.transactionData = new TransactionData();
            }

            /**
             * Sets the Network ID
             *
             * @param Nid Network ID ("0x1" for Mainnet, etc)
             * @return self
             */
            public Builder Nid(BigInteger Nid)
            {
                transactionData.Nid = Nid;
                return this;
            }

            /**
             * Sets the Network ID
             *
             * @param Nid Network ID ("0x1" for Mainnet, etc)
             * @return self
             */
            public Builder Nid(NetworkId Nid)
            {
                transactionData.Nid = (BigInteger)(long)Nid;
                return this;
            }

            /**
             * Sets the sender address
             *
             * @param from EOA address that created the transaction
             * @return self
             */
            public Builder From(Address from)
            {
                transactionData.From = from;
                return this;
            }

            /**
             * Sets the receiver address
             *
             * @param to EOA address to receive coins, or SCORE address to execute the transaction.
             * @return self
             */
            public Builder To(Address to)
            {
                transactionData.To = to;
                return this;
            }

            /**
             * Sets the value to send ICXs
             *
             * @param value Amount of ICX coins in loop to transfer. (1 icx = 1 ^ 18 loop)
             * @return self
             */
            public Builder Value(BigInteger value)
            {
                transactionData.Value = value;
                return this;
            }

            /**
             * Sets the Maximum step
             *
             * @param stepLimit Maximum step allowance that can be used by the transaction.
             * @return self
             */
            public Builder StepLimit(BigInteger stepLimit)
            {
                transactionData.StepLimit = stepLimit;
                return this;
            }

            /**
             * Sets the timestamp
             *
             * @param timestamp Transaction creation time, in microsecond.
             * @return self
             */
            public Builder Timestamp(BigInteger timestamp)
            {
                transactionData.Timestamp = timestamp;
                return this;
            }

            /**
             * Sets the nonce
             *
             * @param nonce An arbitrary number used to prevent transaction hash collision.
             * @return self
             */
            public Builder Nonce(BigInteger nonce)
            {
                transactionData.Nonce = nonce;
                return this;
            }

            /**
             * Converts the builder to CallBuilder with the calling method name
             *
             * @param method calling method name
             * @return {@link CallBuilder}
             */
            public CallBuilder Call(String method)
            {
                return new CallBuilder(transactionData, method);
            }

            /**
             * Converts the builder to DeployBuilder with the deploying content
             *
             * @param contentType content type
             * @param content     deploying content
             * @return {@link DeployBuilder}
             */
            public DeployBuilder Deploy(String contentType, byte[] content)
            {
                return new DeployBuilder(transactionData, contentType, content);
            }

            /**
             * Converts the builder to MessageBuilder with the message
             *
             * @param message message
             * @return {@link MessageBuilder}
             */
            public MessageBuilder Message(String message)
            {
                return new MessageBuilder(transactionData, message);
            }

            /**
             * Make a new transaction using given properties
             *
             * @return a transaction to send
             */
            public ITransaction Build()
            {
                return transactionData.Build();
            }

        }

        /**
         * A Builder for the calling SCORE transaction.
         */
        public class CallBuilder
        {

            private TransactionData _transactionData;
            private RpcObject.Builder _dataBuilder;

            public CallBuilder(TransactionData transactionData, String method)
            {
                this._transactionData = transactionData;
                this._transactionData.DataType = "call";

                _dataBuilder = new RpcObject.Builder()
                        .Put("method", new RpcValue(method));
            }

            /**
             * Sets the params
             *
             * @param params Function parameters
             * @return self
             */
            public CallBuilder Params(RpcObject @params)
            {
                _dataBuilder.Put("params", @params);
                return this;
            }

            /**
             * Sets the params
             *
             * @param params Function parameters
             * @return self
             */
            public CallBuilder Params<T>(T @params)
            {
                _dataBuilder.Put("params", RpcItemCreator.Create(@params));
                return this;
            }

            /**
             * Make a new transaction using given properties
             *
             * @return a transaction to send
             */
            public ITransaction Build()
            {
                _transactionData.Data = _dataBuilder.Build();
                CheckArgument(((RpcObject)_transactionData.Data).GetItem("method"), "method not found");

                return _transactionData.Build();
            }
        }

        /**
         * A Builder for the message transaction.
         */
        public class MessageBuilder
        {
            private TransactionData transactionData;

            public MessageBuilder(TransactionData transactionData, String message)
            {
                this.transactionData = transactionData;
                this.transactionData.DataType = "message";
                this.transactionData.Data = new RpcValue(Encoding.UTF8.GetBytes(message));
            }

            /**
             * Make a new transaction using given properties
             *
             * @return a transaction to send
             */
            public ITransaction Build()
            {
                return transactionData.Build();
            }

        }

        /**
         * A Builder for the deploy transaction.
         */
        public class DeployBuilder
        {
            private TransactionData transactionData;
            private RpcObject.Builder dataBuilder;

            public DeployBuilder(TransactionData transactionData, String contentType, byte[] content)
            {
                this.transactionData = transactionData;
                this.transactionData.DataType = "deploy";

                dataBuilder = new RpcObject.Builder()
                        .Put("contentType", new RpcValue(contentType))
                        .Put("content", new RpcValue(content));
            }

            /**
             * Sets the params
             *
             * @param params Function parameters will be delivered to on_install() or on_update()
             * @return self
             */
            public DeployBuilder Params(RpcObject @params)
            {
                dataBuilder.Put("params", @params);
                return this;
            }

            /**
             * Make a new transaction using given properties
             *
             * @return a transaction to send
             */
            public ITransaction Build()
            {
                transactionData.Data = dataBuilder.Build();
                CheckArgument(((RpcObject)transactionData.Data).GetItem("contentType"), "contentType not found");
                CheckArgument(((RpcObject)transactionData.Data).GetItem("content"), "content not found");

                return transactionData.Build();
            }
        }

        public class TransactionData
        {
            public BigInteger Version = BigInteger.Parse("3");
            public Address From { get; set; }
            public Address To { get; set; }
            public BigInteger? Value { get; set; }
            public BigInteger? StepLimit { get; set; }
            public BigInteger?Timestamp { get; set; }
            public BigInteger? Nid = (BigInteger)(long)NetworkId.Main;
            public BigInteger? Nonce { get; set; }
            public String DataType { get; set; }
            public RpcItem Data { get; set; }

            public ITransaction Build()
            {
                CheckAddress(From, "from not found");
                CheckAddress(To, "to not found");
                CheckArgument(Version, "version not found");
                CheckArgument(StepLimit, "stepLimit not found");
                return new SendingTransaction(this);
            }

            void CheckAddress(Address address, String message)
            {
                CheckArgument(address, message);
                if (address.IsMalformed())
                {
                    throw new ArgumentException("Invalid address");
                }
            }
        }

        private class SendingTransaction : ITransaction
        {
            private BigInteger _version;
            private Address _from;
            private Address _to;
            private BigInteger? _value;
            private BigInteger? _stepLimit;
            private BigInteger? _timestamp;
            private BigInteger? _nid;
            private BigInteger? _nonce;
            private String _dataType;
            private RpcItem _data;

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

            public BigInteger GetVersion()
            {
                return _version;
            }

            public Address GetFrom()
            {
                return _from;
            }

            public Address GetTo()
            {
                return _to;
            }

            public BigInteger? GetValue()
            {
                return _value;
            }

            public BigInteger? GetStepLimit()
            {
                return _stepLimit;
            }

            public BigInteger? GetTimestamp()
            {
                return _timestamp;
            }

            public BigInteger? GetNid()
            {
                return _nid;
            }

            public BigInteger? GetNonce()
            {
                return _nonce;
            }

            public String GetDataType()
            {
                return _dataType;
            }

            public RpcItem GetData()
            {
                return _data;
            }
        }

        public static void CheckArgument<T>(T @object, String message)
        {
            if (@object == null)
            {
                throw new ArgumentException(message);
            }
        }
    }
}
