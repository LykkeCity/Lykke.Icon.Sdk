using Lykke.Icon.Sdk.Transport.JsonRpc;
using System.Numerics;
using System;
using System.Globalization;

namespace Lykke.Icon.Sdk.Data
{

    public class ConfirmedTransaction : Transaction
    {
        private RpcObject properties;

        public ConfirmedTransaction(RpcObject properties)
        {
            this.properties = properties;
        }

        public RpcObject GetProperties()
        {
            return properties;
        }

        public BigInteger GetVersion()
        {
            RpcItem item = properties.GetItem("version");
            return item != null ? item.ToInteger() : BigInteger.Parse("2");
        }

        public Address GetFrom()
        {
            RpcItem item = properties.GetItem("from");
            return item != null ? item.ToAddress() : null;
        }

        public Address GetTo()
        {
            RpcItem item = properties.GetItem("to");
            return item != null ? item.ToAddress() : null;
        }

        public BigInteger? GetFee()
        {
            RpcItem item = properties.GetItem("fee");
            return item != null ? (BigInteger?)ConvertHex(item.ToValue()) : 0;
        }

        public BigInteger? GetValue()
        {
            RpcItem item = properties.GetItem("value");

            if (item == null)
                return null;

            return GetVersion() < 3 ? ConvertHex(item.ToValue()) : item.ToInteger();
        }

        public BigInteger? GetStepLimit()
        {
            RpcItem item = properties.GetItem("stepLimit");
            return item != null ? (BigInteger?)item.ToInteger() : null;
        }

        public BigInteger? GetTimestamp()
        {
            RpcItem item = properties.GetItem("timestamp");

            if (item == null)
                return null;

            return GetVersion() < 3 ? ConvertDecimal(item.ToValue()) : item.ToInteger();
        }

        public BigInteger? GetNid()
        {
            RpcItem item = properties.GetItem("nid");
            return item != null ? item.ToInteger() : 0;
        }

        public BigInteger? GetNonce()
        {
            RpcItem item = properties.GetItem("nonce");

            if (item == null)
                return null;

            return GetVersion() < 3 ? ConvertDecimal(item.ToValue()) : item.ToInteger();
        }

        public String GetDataType()
        {
            RpcItem item = properties.GetItem("dataType");
            return item != null ? item.ToString() : null;
        }

        public RpcItem GetData()
        {
            return properties.GetItem("data");
        }

        public Bytes GetTxHash()
        {
            String key = GetVersion() < 3 ? "tx_hash" : "txHash";
            RpcItem item = properties.GetItem(key);
            return item != null ? item.ToBytes() : null;
        }

        public BigInteger? GetTxIndex()
        {
            RpcItem item = properties.GetItem("txIndex");

            return item != null ? (BigInteger?) item.ToInteger() : null;
        }

        public BigInteger? GetBlockHeight()
        {
            RpcItem item = properties.GetItem("blockHeight");

            return item != null ? (BigInteger?)item.ToInteger() : null;
        }

        public Bytes GetBlockHash()
        {
            RpcItem item = properties.GetItem("blockHash");
            return item != null ? item.ToBytes() : null;
        }

        public String GetSignature()
        {
            RpcItem item = properties.GetItem("signature");
            return item != null ? item.ToString() : null;
        }

        public override String ToString()
        {
            return "ConfirmedTransaction{" +
                    "properties=" + properties +
                    '}';
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

            String stringValue = value.ToString();
            if (stringValue.StartsWith(Bytes.HEX_PREFIX) ||
                    stringValue.StartsWith("-" + Bytes.HEX_PREFIX))
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

            String stringValue = value.ToString();
            String sign = "";
            if (stringValue[0] == '-')
            {
                sign = stringValue.Substring(0, 1);
                stringValue = stringValue.Substring(1);
            }
            return BigInteger.Parse(sign + Bytes.CleanHexPrefix(stringValue), NumberStyles.HexNumber);
        }
    }
}
