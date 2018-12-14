using Lykke.Icon.Sdk.Data;
using System.Numerics;
using System;
using System.Globalization;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
    /**
     * RpcValue contains a leaf value such as string, bytes, integer, boolean
     */
    public class RpcValue : RpcItem
    {
        private String value;

        public RpcValue(RpcValue value)
        {
            this.value = value.ToString();
        }

        public RpcValue(Address value)
        {
            if (value.IsMalformed()) throw new ArgumentException("Invalid address");
            this.value = value.ToString();
        }

        public RpcValue(String value)
        {
            this.value = value;
        }

        public RpcValue(byte[] value)
        {
            this.value = new Bytes(value).ToHexString(true);
        }

        public RpcValue(BigInteger value)
        {
            String sign = (value.Sign == -1) ? "-" : "";
            this.value = sign + Bytes.HEX_PREFIX + BigInteger.Abs(value).ToString("x").TrimStart('0');
        }

        public RpcValue(bool value)
        {
            this.value = value ? "0x1" : "0x0";
        }

        public RpcValue(Bytes value)
        {
            this.value = value.ToString();
        }

        public override bool IsEmpty()
        {
            return value == null || string.IsNullOrEmpty(value);
        }

        /**
         * Returns the value as string
         *
         * @return the value as string
         */
        public override String ToString()
        {
            return value;
        }

        /**
         * Returns the value as bytes
         *
         * @return the value as bytes
         */
        public new byte[] ToByteArray()
        {
            if (!value.StartsWith(Bytes.HEX_PREFIX))
            {
                throw new RpcValueException("The value is not hex string.");
            }

            // bytes should be even length of hex string
            if (value.Length % 2 != 0)
            {
                throw new RpcValueException(
                    "The hex value is not bytes format.");
            }

            return new Bytes(value).ToByteArray();
        }

        public Address ToAddress()
        {
            try
            {
                return new Address(value);
            }
            catch (ArgumentException e)
            {
                if (value == null)
                {
                    return null;
                }
                else
                {
                    return Address.CreateMalformedAddress(value);
                }
            }
        }

        public Bytes ToBytes()
        {
            return new Bytes(value);
        }

        /**
         * Returns the value as integer
         *
         * @return the value as integer
         */
        public BigInteger ToInteger()
        {
            if (!(value.StartsWith(Bytes.HEX_PREFIX) || value.StartsWith('-' + Bytes.HEX_PREFIX)))
            {
                throw new RpcValueException("The value is not hex string.");
            }

            try
            {
                //The dark magic with 0 and f explained in the article below:
                //https://stackoverflow.com/questions/30119174/converting-a-hex-string-to-its-biginteger-equivalent-negates-the-value
                String sign = "0";
                if (value[0] == '-')
                {
                    sign = "";// value.Substring(0, 1);
                    value = value.Substring(1);
                }

                String result = "0" + Bytes.CleanHexPrefix(value);
                var parseResult = BigInteger.Parse(result, NumberStyles.AllowHexSpecifier);

                if (string.IsNullOrEmpty(sign))
                    parseResult *= -1;

                return parseResult;
            }
            catch (Exception e)
            {
                throw new RpcValueException("The value is not hex string.");
            }
        }

        /**
         * Returns the value as boolean
         *
         * @return the value as boolean
         */
        public Boolean ToBoolean()
        {
            switch (value)
            {
                case "0x0":
                    return false;
                case "0x1":
                    return true;
                default:
                    throw new RpcValueException("The value is not boolean format.");
            }
        }
    }
}
