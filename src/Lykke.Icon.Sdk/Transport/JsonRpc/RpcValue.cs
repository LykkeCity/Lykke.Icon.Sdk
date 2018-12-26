using System;
using System.Globalization;
using System.Numerics;
using JetBrains.Annotations;
using Lykke.Icon.Sdk.Data;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
    [PublicAPI]
    public class RpcValue : RpcItem
    {
        private string _value;

        public RpcValue(RpcValue value)
        {
            _value = value.ToString();
        }

        public RpcValue(Address value)
        {
            if (value.IsMalformed())
            {
                throw new ArgumentException("Invalid address");
            }
            
            _value = value.ToString();
        }

        public RpcValue(string value)
        {
            _value = value;
        }

        public RpcValue(byte[] value)
        {
            _value = new Bytes(value).ToHexString(true);
        }

        public RpcValue(BigInteger value)
        {
            var sign = (value.Sign == -1) ? "-" : "";
            
            _value = sign + Bytes.HexPrefix + BigInteger.Abs(value).ToString("x").TrimStart('0');
        }

        public RpcValue(bool value)
        {
            _value = value ? "0x1" : "0x0";
        }

        public RpcValue(Bytes value)
        {
            _value = value.ToString();
        }

        public override bool IsEmpty()
        {
            return _value == null || string.IsNullOrEmpty(_value);
        }

        public override Address ToAddress()
        {
            try
            {
                return new Address(_value);
            }
            catch (ArgumentException)
            {
                return _value == null ? null : Address.CreateMalformedAddress(_value);
            }
        }

        public override byte[] ToByteArray()
        {
            if (!_value.StartsWith(Bytes.HexPrefix))
            {
                throw new RpcValueException("The value is not hex string.");
            }

            // bytes should be even length of hex string
            if (_value.Length % 2 != 0)
            {
                throw new RpcValueException("The hex value is not bytes format.");
            }

            return new Bytes(_value).ToByteArray();
        }

        public override Bytes ToBytes()
        {
            return new Bytes(_value);
        }

        public override BigInteger ToInteger()
        {
            if (!(_value.StartsWith(Bytes.HexPrefix) || _value.StartsWith('-' + Bytes.HexPrefix)))
            {
                throw new RpcValueException("The value is not hex string.");
            }

            try
            {
                //The dark magic with 0 and f explained in the article below:
                //https://stackoverflow.com/questions/30119174/converting-a-hex-string-to-its-biginteger-equivalent-negates-the-value
                
                var sign = "0";
                
                if (_value[0] == '-')
                {
                    sign = "";// value.Substring(0, 1);
                    _value = _value.Substring(1);
                }

                var result = "0" + Bytes.CleanHexPrefix(_value);
                var parseResult = BigInteger.Parse(result, NumberStyles.AllowHexSpecifier);

                if (string.IsNullOrEmpty(sign))
                {
                    parseResult *= -1;
                }

                return parseResult;
            }
            catch (Exception)
            {
                throw new RpcValueException("The value is not hex string.");
            }
        }

        public override bool ToBoolean()
        {
            switch (_value)
            {
                case "0x0":
                    return false;
                case "0x1":
                    return true;
                default:
                    throw new RpcValueException("The value is not boolean format.");
            }
        }

        public override string ToString()
        {
            return _value;
        }
    }
}
