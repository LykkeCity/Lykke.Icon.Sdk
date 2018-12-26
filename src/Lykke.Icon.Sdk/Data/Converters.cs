using System;
using System.Collections.Generic;
using System.Numerics;
using Lykke.Icon.Sdk.Transport.JsonRpc;

namespace Lykke.Icon.Sdk.Data
{
    public static class Converters
    {
        public static readonly IRpcConverter<RpcItem> RpcItem
            = new RpcItemConverter();

        public static readonly IRpcConverter<BigInteger> BigInteger
            = new BigIntegerConverter();

        public static readonly IRpcConverter<bool> Boolean
            = new BoolConverter();

        public static readonly IRpcConverter<string> String
            = new StringConverter();

        public static readonly IRpcConverter<Bytes> Bytes
            = new BytesConverter();

        public static readonly IRpcConverter<byte[]> ByteArray
            = new ByteArrayConverter();

        public static readonly IRpcConverter<Block> Block 
            = new BlockConverter();

        public static readonly IRpcConverter<ConfirmedTransaction> ConfirmedTransaction
            = new ConfirmedTransactionConverter();

        public static readonly IRpcConverter<TransactionResult> TransactionResult
            = new TransactionResultConverter();

        public static readonly IRpcConverter<List<ScoreApi>> ScoreApiList
            = new ListScoreApiConverter();

        
        public static IRpcConverterFactory NewFactory<T>(IRpcConverter<T> converter)
        {
            return new CustomRpcConverterFactory<T>(converter);
        }

        private static object FromRpcItem<T>(T item)
        {
            if (item == null)
            {
                return null;
            }

            if (item.GetType().IsAssignableFrom(typeof(RpcArray)))
            {
                return FromRpcArray<T>(item as RpcArray);
            }

            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (item.GetType().IsAssignableFrom(typeof(RpcObject)))
            {
                return FromRpcObject<T>(item as RpcObject);
            }

            return FromRpcValue<T>(item as RpcValue);
        }

        private static object FromRpcArray<T>(RpcArray array)
        {
            var type = typeof(T);
            
            if (type.IsAssignableFrom(typeof(RpcArray)))
            {
                return array;
            }
            
            var result = new List<object>();
            
            foreach (var item in array)
            {
                var v = FromRpcItem(item);
                
                if (v != null)
                {
                    result.Add(FromRpcItem(item));
                }
            }
            
            return result;
        }

        private static object FromRpcObject<T>(RpcObject @object)
        {
            var type = typeof(T);
            
            if (type.IsAssignableFrom(typeof(RpcObject)))
            {
                return @object;
            }
            
            var result = new Dictionary<string, object>();
            var keys = @object.GetKeys();
            
            foreach (var key in keys)
            {
                var v = FromRpcItem(@object.GetItem(key));
                if (v != null) result[key] = v;
            }
            
            return result;
        }

        private static object FromRpcValue<T>(RpcValue value)
        {
            var type = typeof(T);
            
            if (type.IsAssignableFrom(typeof(bool)))
            {
                return value.ToBoolean();
            }
            else if (type.IsAssignableFrom(typeof(string)))
            {
                return value.ToString();
            }
            else if (type.IsAssignableFrom(typeof(BigInteger)))
            {
                return value.ToInteger();
            }
            else if (type.IsAssignableFrom(typeof(byte[])))
            {
                return value.ToByteArray();
            }
            else if (type.IsAssignableFrom(typeof(Bytes)))
            {
                return value.ToBytes();
            }
            else if (type.IsAssignableFrom(typeof(Address)))
            {
                return value.ToAddress();
            }
            else if (type.IsAssignableFrom(typeof(RpcItem)))
            {
                return value;
            }
            
            return null;
        }
        
        #region NestedClasses

        private sealed class RpcItemConverter : IRpcConverter<RpcItem>
        {
            public RpcItem ConvertTo(RpcItem @object)
            {
                return @object;
            }

            public RpcItem ConvertFrom(RpcItem @object)
            {
                return @object;
            }
        }

        private sealed class BigIntegerConverter : IRpcConverter<BigInteger>
        {
            public BigInteger ConvertTo(RpcItem @object)
            {
                return @object.ToInteger();
            }

            public RpcItem ConvertFrom(BigInteger @object)
            {
                return RpcItemCreator.Create(@object);
            }
        }

        private sealed class BoolConverter : IRpcConverter<bool>
        {
            public bool ConvertTo(RpcItem @object)
            {
                return @object.ToBoolean();
            }

            public RpcItem ConvertFrom(bool @object)
            {
                return RpcItemCreator.Create(@object);
            }
        }

        private sealed class StringConverter : IRpcConverter<string>
        {
            public string ConvertTo(RpcItem @object)
            {
                return @object.ToString();
            }

            public RpcItem ConvertFrom(string @object)
            {
                return RpcItemCreator.Create(@object);
            }
        }

        private sealed class BytesConverter : IRpcConverter<Bytes>
        {
            public Bytes ConvertTo(RpcItem @object)
            {
                return @object.ToBytes();
            }

            public RpcItem ConvertFrom(Bytes @object)
            {
                return RpcItemCreator.Create(@object);
            }
        }

        private sealed class ByteArrayConverter : IRpcConverter<byte[]>
        {
            public byte[] ConvertTo(RpcItem @object)
            {
                return @object.ToByteArray();
            }

            public RpcItem ConvertFrom(byte[] @object)
            {
                return RpcItemCreator.Create(@object);
            }
        }

        private sealed class BlockConverter : IRpcConverter<Block>
        {
            public Block ConvertTo(RpcItem @object)
            {
                return new Block(@object.ToObject());
            }

            public RpcItem ConvertFrom(Block @object)
            {
                return RpcItemCreator.Create(@object);
            }
        }

        private sealed class ConfirmedTransactionConverter : IRpcConverter<ConfirmedTransaction>
        {
            public ConfirmedTransaction ConvertTo(RpcItem @object)
            {
                return new ConfirmedTransaction(@object.ToObject());
            }

            public RpcItem ConvertFrom(ConfirmedTransaction @object)
            {
                return RpcItemCreator.Create(@object);
            }
        }

        private sealed class TransactionResultConverter : IRpcConverter<TransactionResult>
        {
            public TransactionResult ConvertTo(RpcItem @object)
            {
                return new TransactionResult(@object.ToObject());
            }

            public RpcItem ConvertFrom(TransactionResult @object)
            {
                return RpcItemCreator.Create(@object);
            }
        }

        private sealed class ListScoreApiConverter : IRpcConverter<List<ScoreApi>>
        {
            public List<ScoreApi> ConvertTo(RpcItem rpcItem)
            {
                var array = rpcItem.ToArray();
                var scoreApis = new List<ScoreApi>(array.Size());
                for (var i = 0; i < array.Size(); i++)
                {
                    scoreApis.Add(new ScoreApi(array.Get(i).ToObject()));
                }
                return scoreApis;
            }

            public RpcItem ConvertFrom(List<ScoreApi> @object)
            {
                return RpcItemCreator.Create(@object);
            }
        }

        private sealed class CustomRpcConverterFactory<TT> : IRpcConverterFactory
        {
            private readonly IRpcConverter<TT> _converter;
            private readonly Type _typeFor;

            public CustomRpcConverterFactory(IRpcConverter<TT> converter)
            {
                _typeFor = typeof(TT);
                _converter = converter;
            }

            public IRpcConverter<T> Create<T>()
            {
                var type = typeof(T);
                return type.IsAssignableFrom(_typeFor) ? (IRpcConverter<T>)_converter : null;
            }
        }

        #endregion
    }
}
