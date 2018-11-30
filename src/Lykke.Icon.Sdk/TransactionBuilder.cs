using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Icon.Sdk.Data;
using Lykke.Icon.Sdk.Transport.JsonRpc;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

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
         * @deprecated This method can be replaced by {@link #NewBuilderter()}
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
         * @deprecated This method can be replaced by {@link #NewBuilderter()}
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
                transactionData.Nid = BigInteger.ValueOf((long)Nid);
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
                transactionData.from = from;
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
                transactionData.to = to;
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
                transactionData.value = value;
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
                transactionData.stepLimit = stepLimit;
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
                transactionData.timestamp = timestamp;
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
                transactionData.nonce = nonce;
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
            public Transaction Build()
            {
                return transactionData.Build();
            }

        }

        /**
         * A Builder for the calling SCORE transaction.
         */
        public class CallBuilder
        {

            private TransactionData transactionData;
            private RpcObject.Builder dataBuilder;

            public CallBuilder(TransactionData transactionData, String method)
            {
                this.transactionData = transactionData;
                this.transactionData.dataType = "call";

                dataBuilder = new RpcObject.Builder()
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
                dataBuilder.Put("params", @params);
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
                dataBuilder.Put("params", RpcItemCreator.Create(@params));
                return this;
            }

            /**
             * Make a new transaction using given properties
             *
             * @return a transaction to send
             */
            public Transaction Build()
            {
                transactionData.data = dataBuilder.Build();
                CheckArgument(((RpcObject)transactionData.data).GetItem("method"), "method not found");

                return transactionData.Build();
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
                this.transactionData.dataType = "message";
                this.transactionData.data = new RpcValue(Encoding.UTF8.GetBytes(message));
            }

            /**
             * Make a new transaction using given properties
             *
             * @return a transaction to send
             */
            public Transaction Build()
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
                this.transactionData.dataType = "deploy";

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
            public Transaction Build()
            {
                transactionData.data = dataBuilder.Build();
                CheckArgument(((RpcObject)transactionData.data).GetItem("contentType"), "contentType not found");
                CheckArgument(((RpcObject)transactionData.data).GetItem("content"), "content not found");

                return transactionData.Build();
            }
        }

        public class TransactionData
        {
            public BigInteger version = new BigInteger("3");
            public Address from;
            public Address to;
            public BigInteger value;
            public BigInteger stepLimit;
            public BigInteger timestamp;
            public BigInteger Nid = BigInteger.ValueOf((long)NetworkId.MAIN);
            public BigInteger nonce;
            public String dataType;
            public RpcItem data;

            public Transaction Build()
            {
                CheckAddress(from, "from not found");
                CheckAddress(to, "to not found");
                CheckArgument(version, "version not found");
                CheckArgument(stepLimit, "stepLimit not found");
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

        private class SendingTransaction : Transaction
        {
            private BigInteger version;
            private Address from;
            private Address to;
            private BigInteger value;
            private BigInteger stepLimit;
            private BigInteger timestamp;
            private BigInteger Nid;
            private BigInteger nonce;
            private String dataType;
            private RpcItem data;

            public SendingTransaction(TransactionData transactionData)
            {
                version = transactionData.version;
                from = transactionData.from;
                to = transactionData.to;
                value = transactionData.value;
                stepLimit = transactionData.stepLimit;
                timestamp = transactionData.timestamp;
                Nid = transactionData.Nid;
                nonce = transactionData.nonce;
                dataType = transactionData.dataType;
                data = transactionData.data;
            }

            public BigInteger GetVersion()
            {
                return version;
            }

            public Address GetFrom()
            {
                return from;
            }

            public Address GetTo()
            {
                return to;
            }

            public BigInteger GetValue()
            {
                return value;
            }

            public BigInteger GetStepLimit()
            {
                return stepLimit;
            }

            public BigInteger GetTimestamp()
            {
                return timestamp;
            }

            public BigInteger GetNid()
            {
                return Nid;
            }

            public BigInteger GetNonce()
            {
                return nonce;
            }

            public String GetDataType()
            {
                return dataType;
            }

            public RpcItem GetData()
            {
                return data;
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
