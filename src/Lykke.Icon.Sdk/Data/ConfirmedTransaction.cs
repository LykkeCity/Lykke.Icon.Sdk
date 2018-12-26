using System.Globalization;
using System.Numerics;
using JetBrains.Annotations;
using Lykke.Icon.Sdk.Transport.JsonRpc;

namespace Lykke.Icon.Sdk.Data
{

    [PublicAPI]
    public class ConfirmedTransaction : ITransaction
    {
        private readonly RpcObject _properties;

        public ConfirmedTransaction(RpcObject properties)
        {
            _properties = properties;
        }

        public RpcObject GetProperties()
        {
            return _properties;
        }

        public BigInteger GetVersion()
        {
            var item = _properties.GetItem("version");
            
            return item?.ToInteger() ?? BigInteger.Parse("2");
        }

        public Address GetFrom()
        {
            var item = _properties.GetItem("from");
            
            return item?.ToAddress();
        }

        public Address GetTo()
        {
            var item = _properties.GetItem("to");
            
            return item?.ToAddress();
        }

        public BigInteger? GetFee()
        {
            var item = _properties.GetItem("fee");
            
            return item != null ? (BigInteger?)ConvertHex(item.ToValue()) : 0;
        }

        public BigInteger? GetValue()
        {
            var item = _properties.GetItem("value");

            if (item == null)
            {
                return null;
            }

            return GetVersion() < 3 ? ConvertHex(item.ToValue()) : item.ToInteger();
        }

        public BigInteger? GetStepLimit()
        {
            var item = _properties.GetItem("stepLimit");
            
            return item?.ToInteger();
        }

        public BigInteger? GetTimestamp()
        {
            var item = _properties.GetItem("timestamp");

            if (item == null)
            {
                return null;
            }

            return GetVersion() < 3 ? ConvertDecimal(item.ToValue()) : item.ToInteger();
        }

        public BigInteger? GetNid()
        {
            var item = _properties.GetItem("nid");
            
            return item?.ToInteger() ?? 0;
        }

        public BigInteger? GetNonce()
        {
            var item = _properties.GetItem("nonce");

            if (item == null)
            {
                return null;
            }

            return GetVersion() < 3 ? ConvertDecimal(item.ToValue()) : item.ToInteger();
        }

        public string GetDataType()
        {
            var item = _properties.GetItem("dataType");
            
            return item?.ToString();
        }

        public RpcItem GetData()
        {
            return _properties.GetItem("data");
        }

        public Bytes GetTxHash()
        {
            var key = GetVersion() < 3 ? "tx_hash" : "txHash";
            var item = _properties.GetItem(key);
            
            return item?.ToBytes();
        }

        public BigInteger? GetTxIndex()
        {
            var item = _properties.GetItem("txIndex");

            return item?.ToInteger();
        }

        public BigInteger? GetBlockHeight()
        {
            var item = _properties.GetItem("blockHeight");

            return item?.ToInteger();
        }

        public Bytes GetBlockHash()
        {
            var item = _properties.GetItem("blockHash");
            
            return item?.ToBytes();
        }

        public string GetSignature()
        {
            var item = _properties.GetItem("signature");
            
            return item?.ToString();
        }

        public override string ToString()
        {
            return "ConfirmedTransaction{properties=" + _properties + '}';
        }

        private BigInteger ConvertDecimal(RpcValue value)
        {
            // The value of timestamp and nonce in v2 specs is a decimal string.
            // But there are decimal strings, numbers and 0x included hex strings in v2 blocks.
            // e.g.) "12345", 12345, "0x12345"
            //
            // RpcValue class converts numbers and 0x included hex strings to 0x included hex string
            // and holds it
            //
            // So, stringValue is a decimal string or a 0x included hex string.("12345", "0x12345")
            // if it has 0x, the method converts it as hex otherwise decimal

            var stringValue = value.ToString();
            
            if (stringValue.StartsWith(Bytes.HexPrefix) || stringValue.StartsWith("-" + Bytes.HexPrefix))
            {
                return ConvertHex(value);
            }
            else
            {
                return BigInteger.Parse(stringValue);
            }
        }

        private BigInteger ConvertHex(RpcValue value)
        {
            // The value of 'value' and nonce in v2 specs is a decimal string.
            // But there are hex strings without 0x in v2 blocks.
            //
            // This method converts the value as hex no matter it has  0x prefix or not.

            var stringValue = value.ToString();
            var sign = "";
            
            if (stringValue[0] == '-')
            {
                sign = stringValue.Substring(0, 1);
                stringValue = stringValue.Substring(1);
            }
            
            return BigInteger.Parse(sign + Bytes.CleanHexPrefix(stringValue), NumberStyles.HexNumber);
        }
    }
}
