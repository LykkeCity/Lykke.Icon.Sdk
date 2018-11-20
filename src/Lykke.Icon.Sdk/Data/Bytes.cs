using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Icon.Sdk.Crypto;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Data
{
    /**
     * A wrapper class of byte array
     */
    public class Bytes
    {
        public static String HEX_PREFIX = "0x";

        private byte[] data;

        /**
         * Creates an instance using hex string
         *
         * @param hexString hex string of bytes
         */
        public Bytes(String hexString)
        {
            if (!IsValidHex(hexString))
                throw new ArgumentException("The value is not hex string.");
            this.data = Hex.Decode(CleanHexPrefix(hexString));
        }

        /**
         * Creates an instance using byte array
         *
         * @param data byte array to wrap
         */
        public Bytes(byte[] data)
        {
            this.data = data;
        }

        public Bytes(BigInteger value)
        {
            this.data = value.ToByteArray();
        }

        /**
         * Gets the data as a byte array
         *
         * @return byte array
         */
        public byte[] ToByteArray()
        {
            return data;
        }

        /**
         * add the pad bytes to the passed in block, returning the
         * number of bytes added.
         */
        private static byte[] ToBytesPadded(BigInteger value, int length)
        {
            byte[] result = new byte[length];
            byte[] bytes = value.ToByteArray();

            int bytesLength;
            int srcOffset;
            if (bytes[0] == 0)
            {
                bytesLength = bytes.Length - 1;
                srcOffset = 1;
            }
            else
            {
                bytesLength = bytes.Length;
                srcOffset = 0;
            }

            if (bytesLength > length)
            {
                throw new ArgumentException("Input is too large to put in byte array of size " + length);
            }

            int destOffset = length - bytesLength;
            System.arraycopy(bytes, srcOffset, result, destOffset, bytesLength);
            return result;
        }

        public static String CleanHexPrefix(String input)
        {
            if (ContainsHexPrefix(input))
            {
                return input.Substring(2);
            }
            else
            {
                return input;
            }
        }

        /**
         * Gets the data as a hex string
         *
         * @param withPrefix whether 0x prefix included
         * @return hex string
         */
        public String ToHexString(boolean withPrefix)
        {
            return ToHexString(withPrefix, data.length);
        }

        public static bool ContainsHexPrefix(String input)
        {
            return input.length() > 1 && input.CharAt(0) == '0' && input.CharAt(1) == 'x';
        }

        /**
         * Gets the data as a byte array given size
         *
         * @param size size of byte array
         * @return byte array given size
         */
        public byte[] ToByteArray(int size)
        {
            return ToBytesPadded(new BigInteger(data), size);
        }

        public String ToString()
        {
            return ToHexString(true, data.Length);
        }

        public bool Equals(Object obj)
        {
            if (obj == this) return true;
            if (obj is Bytes)
            {
                return Arrays.Equals(((Bytes)obj).data, data);
            }
            return false;
        }

        /**
         * Gets the data as a hex string given size
         *
         * @param withPrefix whether 0x prefix included
         * @param size size of byte array
         * @return hex string given size
         */
        public String ToHexString(boolean withPrefix, int size)
        {
            String result = Hex.ToHexString(data);
            int length = result.Length;
            if (length < size)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < size - length; i++)
                {
                    sb.append('0');
                }
                result = sb.Append(result).ToString();
            }

            if (withPrefix)
            {
                return "0x" + result;
            }
            else
            {
                return result;
            }
        }

        public int Length()
        {
            return data == null ? 0 : data.Length;
        }

        private boolean IsValidHex(String value)
        {
            String v = CleanHexPrefix(value);
            return v.Matches("^[0-9a-fA-F]+$");
        }
    }
}