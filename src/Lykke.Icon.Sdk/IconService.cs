using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Icon.Sdk;
using Lykke.Icon.Sdk.Crypto;
using Lykke.Icon.Sdk.Data;
using Lykke.Icon.Sdk.Transport.JsonRpc;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk
{

    /**
     * IconService which provides APIs of ICON network.
     */
    public class IconService
    {
        private Provider provider;
        private List<RpcConverterFactory> converterFactories = new List<RpcConverterFactory>(10);
        private Dictionary<Type, RpcConverter<object>> converterMap = new Dictionary<Type, RpcConverter<object>>();

        /**
         * Creates IconService instance
         *
         * @param provider the worker that transporting requests
         */
        public IconService(Provider provider)
        {
            this.provider = provider;
            AddConverterFactory(Converters.NewFactory(Converters.BIG_INTEGER));
            AddConverterFactory(Converters.NewFactory(Converters.BOOLEAN));
            AddConverterFactory(Converters.NewFactory(Converters.STRING));
            AddConverterFactory(Converters.NewFactory(Converters.BYTES));
            AddConverterFactory(Converters.NewFactory(Converters.BYTE_ARRAY));
            AddConverterFactory(Converters.NewFactory(Converters.BLOCK));
            AddConverterFactory(Converters.NewFactory(Converters.CONFIRMED_TRANSACTION));
            AddConverterFactory(Converters.NewFactory(Converters.TRANSACTION_RESULT));
            AddConverterFactory(Converters.NewFactory(Converters.SCORE_API_LIST));
            AddConverterFactory(Converters.NewFactory(Converters.RPC_ITEM));
        }

        /**
         * Get the total number of issued coins.
         *
         * @return A BigNumber instance of the total number of coins.
         */
        public Request<BigInteger> GetTotalSupply()
        {
            var requestId = GetCurrentUnixTime();
            var request = new Request(requestId, "icx_getTotalSupply", null);
            return provider.Request(request, FindConverter<BigInteger>());
        }

        private static long GetCurrentUnixTime()
        {
            DateTime foo = DateTime.UtcNow;
            long requestId = ((DateTimeOffset)foo).ToUnixTimeSeconds();
            return requestId;
        }

        /**
         * Get the balance of an address.
         *
         * @param address The address to get the balance of.
         * @return A BigNumber instance of the current balance for the given address in loop.
         */
        public Request<BigInteger> GetBalance(Address address)
        {
            var requestId = GetCurrentUnixTime();
            RpcObject @params = new RpcObject.Builder()
                    .Put("address", new RpcValue(address))
                    .Build();
            var request = new Request(requestId, "icx_getBalance", @params);
            return provider.Request(request, FindConverter<BigInteger>());
        }

        /**
         * Get a block matching the block number.
         *
         * @param height The block number
         * @return The Block object
         */
        public Request<Block> GetBlock(BigInteger height)
        {
            var requestId = GetCurrentUnixTime();
            RpcObject @params = new RpcObject.Builder()
                    .Put("height", new RpcValue(height))
                    .Build();
            var request = new Request(requestId, "icx_getBlockByHeight", @params);
            return provider.Request(request, FindConverter<Block>());
        }

        /**
         * Get a block matching the block hash.
         *
         * @param hash The block hash (without hex prefix) or the string 'latest'
         * @return The Block object
         */
        public Request<Block> GetBlock(Bytes hash)
        {
            var requestId = GetCurrentUnixTime();
            RpcObject @params = new RpcObject.Builder()
                    .Put("hash", new RpcValue(hash))
                    .Build();
            var request = new Request(requestId, "icx_getBlockByHash", @params);
            return provider.Request(request, FindConverter<Block>());
        }


        /**
         * Get the latest block.
         *
         * @return The Block object
         */
        public Request<Block> GetLastBlock()
        {
            var requestId = GetCurrentUnixTime();
            var request = new Request(requestId, "icx_getLastBlock", null);
            return provider.Request(request, FindConverter<Block>());
        }

        /**
         * Get information about api function in score
         *
         * @param scoreAddress The address to get APIs
         * @return The ScoreApi object
         */
        public Request<List<ScoreApi>> GetScoreApi(Address scoreAddress)
        {
            if (!IconKeys.IsContractAddress(scoreAddress))
                throw new ArgumentException("Only the contract address can be called.");
            var requestId = GetCurrentUnixTime();
            RpcObject @params = new RpcObject.Builder()
                    .Put("address", new RpcValue(scoreAddress))
                    .Build();
            var request = new Request(requestId, "icx_getScoreApi", @params);
            return provider.Request(request, FindConverter<List<ScoreApi>>());
        }


        /**
         * Get a transaction matching the given transaction hash.
         *
         * @param hash The transaction hash
         * @return The Transaction object
         */
        public Request<ConfirmedTransaction> GetTransaction(Bytes hash)
        {
            var requestId = GetCurrentUnixTime();
            RpcObject @params = new RpcObject.Builder()
                    .Put("txHash", new RpcValue(hash))
                    .Build();
            var request = new Request(requestId, "icx_getTransactionByHash", @params);
            return provider.Request(request, FindConverter<ConfirmedTransaction>());
        }

        /**
         * Get the result of a transaction by transaction hash.
         *
         * @param hash The transaction hash
         * @return The TransactionResult object
         */
        public Request<TransactionResult> GetTransactionResult(Bytes hash)
        {
            var requestId = GetCurrentUnixTime();
            RpcObject @params = new RpcObject.Builder()
                    .Put("txHash", new RpcValue(hash))
                    .Build();
            var request = new Request(requestId, "icx_getTransactionResult", @params);
            return provider.Request(request, FindConverter<TransactionResult>());
        }

        /**
         * Calls a SCORE API just for reading
         *
         * @param call instance of Call
         * @param <O> responseType
         * @return the Request object can execute a request
         */
        public Request<O> Call<O>(Call<O> call)
        {
            var requestId = GetCurrentUnixTime();
            var request = new Request(requestId, "icx_call", call.GetProperties());
            return provider.Request(request, FindConverter<O>());
        }

        /**
         * Sends a transaction that changes the states of account
         *
         * @param signedTransaction parameters including signatures
         * @return the Request object can execute a request (result type is txHash)
         */
        public Request<Bytes> SendTransaction(SignedTransaction signedTransaction)
        {
            var requestId = GetCurrentUnixTime();
            var request = new Request(
                    requestId, "icx_sendTransaction", signedTransaction.GetProperties());
            return provider.Request(request, FindConverter<Bytes>());
        }

        private RpcConverter<T> FindConverter<T>()
        {
            var type = typeof(T);
            RpcConverter<object> converterObj = converterMap[type];
            if (converterObj is RpcConverter<T> converter) return converter;

            foreach (RpcConverterFactory factory in converterFactories)
            {
                converter = factory.Create<T>();
                if (converter != null)
                {
                    converterMap[type] = (RpcConverter<object>)converter;
                    return converter;
                }
            }

            //if (type.isAnnotationPresent(AnnotationConverter.class)) {
            //        if (type.getAnnotation(AnnotationConverter.class).use()) {
            //            return new AnnotatedConverterFactory().create(type);
            //        }
            //    }

            throw new ArgumentException("Could not locate response converter for:'" + type + "'");
        }

        /**
         * Adds Converter factory.
         * It has a create function that creates the converter of the specific type.
         *
         * @param factory a converter factory
         */
        public void AddConverterFactory(RpcConverterFactory factory)
        {
            converterFactories.Add(factory);
        }
    }
}
