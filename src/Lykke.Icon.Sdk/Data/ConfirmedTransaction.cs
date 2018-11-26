using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Icon.Sdk.Crypto;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Data
{

public class ConfirmedTransaction : Transaction {

    private RpcObject properties;

    ConfirmedTransaction(RpcObject properties) {
        this.properties = properties;
    }

    public RpcObject GetProperties() {
        return properties;
    }

    public override BigInteger GetVersion() {
        RpcItem item = properties.GetItem("version");
        return item != null ? item.ToInteger() : BigInteger.valueOf(2);
    }

    public override Address GetFrom() {
        RpcItem item = properties.GetItem("from");
        return item != null ? item.ToAddress() : null;
    }

    public override Address GetTo() {
        RpcItem item = properties.GetItem("to");
        return item != null ? item.ToAddress() : null;
    }

    public BigInteger GetFee() {
        RpcItem item = properties.GetItem("fee");
        return item != null ? ConvertHex(item.ToValue()) : null;
    }

    public override BigInteger GetValue() {
        RpcItem item = properties.GetItem("value");
        if (item == null) {
            return null;
        }
        return GetVersion().intValue() < 3 ? ConvertHex(item.ToValue()) : item.ToInteger();
    }

    public override BigInteger GetStepLimit() {
        RpcItem item = properties.GetItem("stepLimit");
        return item != null ? item.ToInteger() : null;
    }

    public override BigInteger GetTimestamp() {
        RpcItem item = properties.GetItem("timestamp");
        if (item == null) {
            return null;
        }
        return GetVersion().intValue() < 3 ? ConvertDecimal(item.ToValue()) : item.ToInteger();
    }

    public override BigInteger GetNid() {
        RpcItem item = properties.GetItem("nid");
        return item != null ? item.ToInteger() : null;
    }

    public override BigInteger GetNonce() {
        RpcItem item = properties.GetItem("nonce");
        if (item == null) {
            return null;
        }
        return GetVersion().intValue() < 3 ? ConvertDecimal(item.ToValue()) : item.ToInteger();
    }

    public override String GetDataType() {
        RpcItem item = properties.GetItem("dataType");
        return item != null ? item.ToString() : null;
    }

    public override RpcItem getData() {
        return properties.GetItem("data");
    }

    public Bytes GetTxHash() {
        String key = GetVersion().intValue() < 3 ? "tx_hash" : "txHash";
        RpcItem item = properties.GetItem(key);
        return item != null ? item.ToBytes() : null;
    }

    public BigInteger GetTxIndex() {
        RpcItem item = properties.GetItem("txIndex");
        return item != null ? item.ToInteger() : null;
    }

    public BigInteger GetBlockHeight() {
        RpcItem item = properties.GetItem("blockHeight");
        return item != null ? item.ToInteger() : null;
    }

    public Bytes GetBlockHash() {
        RpcItem item = properties.GetItem("blockHash");
        return item != null ? item.ToBytes() : null;
    }

    public String GetSignature() {
        RpcItem item = properties.GetItem("signature");
        return item != null ? item.ToString() : null;
    }

    public override String ToString() {
        return "ConfirmedTransaction{" +
                "properties=" + properties +
                '}';
    }

    private BigInteger ConvertDecimal(RpcValue value) {
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
        if (stringValue.startsWith(Bytes.HEX_PREFIX) ||
                stringValue.StartsWith("-" + Bytes.HEX_PREFIX)) {
            return ConvertHex(value);
        } else {
            return new BigInteger(stringValue, 10);
        }
    }

    private BigInteger ConvertHex(RpcValue value) {
        // The value of 'value' and nonce in v2 specs is a decimal string.
        // But there are hex strings without 0x in v2 blocks.
        //
        // This method converts the value as hex no matter it has  0x prefix or not.

        String stringValue = value.ToString();
        String sign = "";
        if (stringValue.CharAt(0) == '-') {
            sign = stringValue.Substring(0, 1);
            stringValue = stringValue.Substring(1);
        }
        return new BigInteger(sign + Bytes.cleanHexPrefix(stringValue), 16);
    }
    }
}
