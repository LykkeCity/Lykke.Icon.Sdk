using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Icon.Sdk.Crypto;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Data
{
    public sealed class Converters
    {
        private Converters()
        {
        }

        #region NestedClasses

        public sealed class RpcItemConverter : RpcConverter<RpcItem>
        {
            public override RpcItem ConvertTo(RpcItem @object)
            {
                return @object;
            }

            public override RpcItem ConvertFrom(RpcItem @object)
            {
                return @object;
            }
        }

        public sealed class BigIntegerConverter : RpcConverter<BigInteger>
        {
            public override BigInteger ConvertTo(RpcItem @object)
            {
                return @object.ToInteger();
            }

            public override RpcItem ConvertFrom(BigInteger @object)
            {
                return RpcItemCreator.Create(@object);
            }
        }

        public sealed class BoolConverter : RpcConverter<bool>
        {
            public override bool ConvertTo(RpcItem @object)
            {
                return object.ToBoolean();
            }

            public override RpcItem ConvertFrom(bool @object)
            {
                return RpcItemCreator.Create(@object);
            }
        }

        public sealed class StringConverter : RpcConverter<string>
        {
            public override String ConvertTo(RpcItem @object)
            {
                return @object.ToString();
            }

            public override RpcItem ConvertFrom(String @object)
            {
                return RpcItemCreator.Create(@object);
            }
        }

        public sealed class BytesConverter : RpcConverter<Bytes>
        {
            public override Bytes ConvertTo(RpcItem @object)
            {
                return object.ToBytes();
            }

            public override RpcItem ConvertFrom(Bytes @object)
            {
                return RpcItemCreator.Create(@object);
            }
        }

        public sealed class ByteArrayConverter : RpcConverter<byte[]>
        {
            public override byte[] ConvertTo(RpcItem @object)
            {
                return object.ToByteArray();
            }

            public override RpcItem ConvertFrom(byte[] @object)
            {
                return RpcItemCreator.Create(@object);
            }
        }

        public sealed class BlockConverter : RpcConverter<Block>
        {
            public override Block ConvertTo(RpcItem @object)
            {
                return new Block(object.ToObject());
            }

            public override RpcItem ConvertFrom(Block @object)
            {
                return RpcItemCreator.Create(@object);
            }
        }

        public sealed class ConfirmedTransactionConverter : RpcConverter<ConfirmedTransaction>
        {
            public override ConfirmedTransaction ConvertTo(RpcItem @object)
            {
                return new ConfirmedTransaction(@object.ToObject());
            }

            public override RpcItem ConvertFrom(ConfirmedTransaction @object)
            {
                return RpcItemCreator.Create(@object);
            }
        }

        public sealed class TransactionResultConverter : RpcConverter<TransactionResult>
        {
            public override TransactionResult ConvertTo(RpcItem @object)
            {
                return new TransactionResult(@object.ToObject());
            }

            public override RpcItem ConvertFrom(TransactionResult @object)
            {
                return RpcItemCreator.Create(@object);
            }
        }

        public sealed class ListScoreApiConverter : RpcConverter<List<ScoreApi>>
        {
            public override List<ScoreApi> ConvertTo(RpcItem rpcItem)
            {
                RpcArray array = rpcItem.asArray();
                List<ScoreApi> scoreApis = new ArrayList<>(array.size());
                for (int i = 0; i < array.size(); i++)
                {
                    scoreApis.add(new ScoreApi(array.get(i).ToObject()));
                }
                return scoreApis;
            }

            public override RpcItem ConvertFrom(List<ScoreApi> @object)
            {
                return RpcItemCreator.Create(@object);
            }
        }

        public sealed class CustomRpcConverterFactory<TT> : RpcConverterFactory
        {
            private readonly RpcConverter<TT> _converter;
            private readonly Type _typeFor;

            public CustomRpcConverterFactory(RpcConverter<TT> converter)
            {
                _typeFor = typeof(TT);
                _converter = converter;
            }

            public override RpcConverter<T> Create<T>()
            {
                var type = typeof(T);
                return type.IsAssignableFrom(_typeFor) ? (RpcConverter<T>)_converter : null;
            }
        }

        #endregion

        public static RpcConverter<RpcItem> RPC_ITEM
            = new RpcItemConverter();

        public static RpcConverter<BigInteger> BIG_INTEGER
            = new BigIntegerConverter();

        public static RpcConverter<bool> BOOLEAN
            = new BoolConverter();

        public static RpcConverter<String> STRING
            = new StringConverter();

        public static RpcConverter<Bytes> BYTES
            = new BytesConverter();

        public static RpcConverter<byte[]> BYTE_ARRAY
            = new ByteArrayConverter();

        public static RpcConverter<Block> BLOCK = new BlockConverter();

        public static RpcConverter<ConfirmedTransaction> CONFIRMED_TRANSACTION
            = new ConfirmedTransactionConverter();

        public static RpcConverter<TransactionResult> TRANSACTION_RESULT
            = new TransactionResultConverter();

        public static RpcConverter<List<ScoreApi>> SCORE_API_LIST
            = new ListScoreApiConverter();

        public static RpcConverterFactory NewFactory<TT>(RpcConverter<TT> converter)
        {
            return new CustomRpcConverterFactory<TT>(converter);
        }

        public static Object FromRpcItem<T>(T item, Type type)
        {
            if (item == null) return null;

            if (item.GetType().IsAssignableFrom(typeof(RpcArray)))
            {
                return fromRpcArray((RpcArray)item, type);
            }

            if (item.GetType().IsAssignableFrom(typeof(RpcObject)))
            {
                return fromRpcObject((RpcObject)item, type);
            }

            return fromRpcValue((RpcValue)item, type);
        }

        static Object FromRpcArray<T>(RpcArray array)
        {
            var type = typeof(T);
            if (type.IsAssignableFrom(typeof(RpcArray))) return array;
            List result = new ArrayList<T>();
            foreach (RpcItem item in array)
            {
                Object v = FromRpcItem(item, type);
                if (v != null) result.add(FromRpcItem(item, type));
            }
            return result;
        }

        static Object FromRpcObject<T>(RpcObject @object)
        {
            var type = typeof(T);
            if (type.IsAssignableFrom(typeof(RpcObject))) return @object;
            Map result = new HashMap();
            Set<String> keys = @object.keySet();
            foreach (String key in keys)
            {
                Object v = FromRpcItem(@object.GetItem(key), type);
                if (v != null) result.put(key, v);
            }
            return result;
        }

        static Object FromRpcValue<T>(RpcValue value)
        {
            var type = typeof(T);
            if (type.IsAssignableFrom(typeof(Boolean)))
            {
                return value.ToBoolean();
            }
            else if (type.IsAssignableFrom(typeof(String)))
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
    }
}
