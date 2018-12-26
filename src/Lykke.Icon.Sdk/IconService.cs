using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Icon.Sdk.Crypto;
using Lykke.Icon.Sdk.Data;
using Lykke.Icon.Sdk.Transport.JsonRpc;

namespace Lykke.Icon.Sdk
{
    /// <inheritdoc />
    [PublicAPI]
    public class IconService : IIconService
    {
        private readonly List<IRpcConverterFactory> _converterFactories;
        private readonly Dictionary<Type, object> _converterMap;
        private readonly IProvider _provider;

        
        /// <summary>
        ///    Creates IconService instance 
        /// </summary>
        /// <param name="provider">
        ///    The worker that transporting requests
        /// </param>
        public IconService(IProvider provider)
        {
            _converterFactories = new List<IRpcConverterFactory>(10);
            _converterMap = new Dictionary<Type, object>();
            _provider = provider;
            
            AddConverterFactory(Converters.BigInteger);
            AddConverterFactory(Converters.Boolean);
            AddConverterFactory(Converters.String);
            AddConverterFactory(Converters.Bytes);
            AddConverterFactory(Converters.ByteArray);
            AddConverterFactory(Converters.Block);
            AddConverterFactory(Converters.ConfirmedTransaction);
            AddConverterFactory(Converters.TransactionResult);
            AddConverterFactory(Converters.ScoreApiList);
            AddConverterFactory(Converters.RpcItem);
        }
        
        
        public void AddConverterFactory<T>(IRpcConverter<T> converter)
        {
            var factory = Converters.NewFactory(converter);
            
            _converterFactories.Add(factory);
            _converterMap.Add(typeof(T), factory.Create<T>());
        }
        
        public void AddConverterFactory<T>(IRpcConverterFactory factory)
        {
            _converterFactories.Add(factory);
            _converterMap.Add(typeof(T), factory.Create<T>());
        }

        /// <inheritdoc />
        public Task<BigInteger> GetBalance(Address address)
        {
            var requestId = GetCurrentUnixTime();
            var requestParams = new RpcObject.Builder()
                .Put("address", new RpcValue(address))
                .Build();
            
            var request = new Request(requestId, "icx_getBalance", requestParams);
            
            return _provider.SendRequestAsync(request, FindConverter<BigInteger>());
        }

        /// <inheritdoc />
        public Task<Block> GetBlock(BigInteger height)
        {
            var requestId = GetCurrentUnixTime();
            var requestParams = new RpcObject.Builder()
                .Put("height", new RpcValue(height))
                .Build();
            
            var request = new Request(requestId, "icx_getBlockByHeight", requestParams);
            
            return _provider.SendRequestAsync(request, FindConverter<Block>());
        }
        
        /// <inheritdoc />
        public Task<Block> GetBlock(Bytes hash)
        {
            var requestId = GetCurrentUnixTime();
            var requestParams = new RpcObject.Builder()
                .Put("hash", new RpcValue(hash))
                .Build();
            
            var request = new Request(requestId, "icx_getBlockByHash", requestParams);
            
            return _provider.SendRequestAsync(request, FindConverter<Block>());
        }
        
        /// <inheritdoc />
        public Task<T> CallAsync<T>(Call<T> call)
        {
            var requestId = GetCurrentUnixTime();
            
            var request = new Request(requestId, "icx_call", call.GetProperties());
            
            return _provider.SendRequestAsync(request, FindConverter<T>());
        }

        /// <inheritdoc />
        public Task<Block> GetLastBlock()
        {
            var requestId = GetCurrentUnixTime();
            
            var request = new Request(requestId, "icx_getLastBlock", null);
            
            return _provider.SendRequestAsync(request, FindConverter<Block>());
        }
        
        /// <inheritdoc />
        public Task<List<ScoreApi>> GetScoreApi(Address scoreAddress)
        {
            if (!IconKeys.IsContractAddress(scoreAddress))
            {
                throw new ArgumentException("Only the contract address can be called.");
            }
            
            var requestId = GetCurrentUnixTime();
            var requestParams = new RpcObject.Builder()
                .Put("address", new RpcValue(scoreAddress))
                .Build();
            
            var request = new Request(requestId, "icx_getScoreApi", requestParams);
            
            return _provider.SendRequestAsync(request, FindConverter<List<ScoreApi>>());
        }
        
        /// <inheritdoc />
        public Task<BigInteger> GetTotalSupply()
        {
            var requestId = GetCurrentUnixTime();
            
            var request = new Request(requestId, "icx_getTotalSupply", null);
            
            return _provider.SendRequestAsync(request, FindConverter<BigInteger>());
        }

        /// <inheritdoc />
        public Task<ConfirmedTransaction> GetTransaction(Bytes hash)
        {
            var requestId = GetCurrentUnixTime();
            var requestParams = new RpcObject.Builder()
                    .Put("txHash", new RpcValue(hash))
                    .Build();
            
            var request = new Request(requestId, "icx_getTransactionByHash", requestParams);
            
            return _provider.SendRequestAsync(request, FindConverter<ConfirmedTransaction>());
        }

        /// <inheritdoc />
        public Task<TransactionResult> GetTransactionResult(Bytes hash)
        {
            var requestId = GetCurrentUnixTime();
            var requestParams = new RpcObject.Builder()
                    .Put("txHash", new RpcValue(hash))
                    .Build();
            
            var request = new Request(requestId, "icx_getTransactionResult", requestParams);
            
            return _provider.SendRequestAsync(request, FindConverter<TransactionResult>());
        }

        /// <inheritdoc />
        public Task<Bytes> SendTransaction(SignedTransaction signedTransaction)
        {
            var requestId = GetCurrentUnixTime();
            
            var request = new Request(requestId, "icx_sendTransaction", signedTransaction.GetProperties());
            
            return _provider.SendRequestAsync(request, FindConverter<Bytes>());
        }

        private IRpcConverter<T> FindConverter<T>()
        {
            var converterType = typeof(T);
            
            _converterMap.TryGetValue(converterType, out var converterObj);

            if (converterObj is IRpcConverter<T> converter)
            {
                return converter;
            }

            foreach (var factory in _converterFactories)
            {
                converter = factory.Create<T>();
                
                if (converter != null)
                {
                    _converterMap[converterType] = converter;
                    
                    return converter;
                }
            }

            throw new ArgumentException("Could not locate response converter for:'" + converterType + "'");
        }
        
        private static long GetCurrentUnixTime()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
}
