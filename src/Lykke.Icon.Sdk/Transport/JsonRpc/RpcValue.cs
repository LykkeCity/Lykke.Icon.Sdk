using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
/**
 * RpcValue contains a leaf value such as string, bytes, integer, boolean
 */
    public class RpcValue : RpcItem
    {
        private String _value;

        public RpcValue(RpcValue value)
        {
            this.value = value.ToString();
        }

        public RpcValue(Address value)
        {
            if (value.IsMalformed()) throw new IllegalArgumentException("Invalid address");
            this.value = value.ToString();
        }

        public RpcValue(String value)
        {
            this.value = value;
        }

        public RpcValue(byte[] value)
        {
            this.value = new Bytes(value).toHexString(true);
        }

        public RpcValue(BigInteger value)
        {
            String sign = (value.signum() == -1) ? "-" : "";
            this.value = sign + HEX_PREFIX + value.abs().toString(16);
        }

        public RpcValue(boolean value)
        {
            this.value = value ? "0x1" : "0x0";
        }

        public RpcValue(Boolean value)
        {
            this(value.booleanValue());
        }

        public RpcValue(Bytes value)
        {
            this.value = value.toString();
        }

        public boolean IsEmpty()
        {
            return value == null || value.IsEmpty();
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
        public byte[] ToByteArray()
        {
            if (!value.startsWith(HEX_PREFIX))
            {
                throw new RpcValueException("The value is not hex string.");
            }

            // bytes should be even length of hex string
            if (value.length() % 2 != 0)
            {
                throw new RpcValueException(
                    "The hex value is not bytes format.");
            }

            return new Bytes(value).toByteArray();
        }

        public Address ToAddress()
        {
            try
            {
                return new Address(value);
            }
            catch (IllegalArgumentException e)
            {
                if (value == null)
                {
                    return null;
                }
                else
                {
                    return Address.createMalformedAddress(value);
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
            if (!(value.startsWith(HEX_PREFIX) || value.startsWith('-' + HEX_PREFIX)))
            {
                throw new RpcValueException("The value is not hex string.");
            }

            try
            {
                String sign = "";
                if (value.charAt(0) == '-')
                {
                    sign = value.substring(0, 1);
                    value = value.substring(1);
                }

                String result = sign + Bytes.cleanHexPrefix(value);
                return new BigInteger(result, 16);
            }
            catch (NumberFormatException e)
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
