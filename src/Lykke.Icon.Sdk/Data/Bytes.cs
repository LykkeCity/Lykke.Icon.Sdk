using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Data
{
    /// <summary>
    ///    A wrapper class of byte array 
    /// </summary>
    [PublicAPI]
    public class Bytes
    {
        public const string HexPrefix = "0x";

        private readonly byte[] _data;

        
        public Bytes(string hexString)
        {
            if (!IsValidHex(hexString))
                throw new ArgumentException("The value is not hex string.");
            _data = Hex.Decode(CleanHexPrefix(hexString));
        }

        public Bytes(byte[] data)
        {
            _data = data;
        }

        public Bytes(BigInteger value)
        {
            _data = value.ToByteArray();
        }

        public int Length()
        {
            return _data?.Length ?? 0;
        }

        /// <summary>
        ///    Gets the data as a byte array 
        /// </summary>
        public byte[] ToByteArray()
        {
            return _data;
        }

        /// <summary>
        ///    Gets the data as a byte array given size
        /// </summary>
        public byte[] ToByteArray(int size)
        {
            return ToBytesPadded(new BigInteger(_data), size);
        }

        /// <summary>
        ///    Gets the data as a hex string 
        /// </summary>
        public string ToHexString(bool withPrefix)
        {
            return ToHexString(withPrefix, _data.Length);
        }

        /// <summary>
        ///    Gets the data as a hex string given size
        /// </summary>
        public string ToHexString(bool withPrefix, int size)
        {
            var result = Hex.ToHexString(_data);
            var length = result.Length;
            if (length < size)
            {
                var sb = new StringBuilder();
                for (var i = 0; i < size - length; i++)
                {
                    sb.Append('0');
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
        
        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            
            if (obj is Bytes bytes)
            {
                return bytes._data.SequenceEqual(_data);
            }
            
            return false;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return ToHexString(true, _data.Length);
        }

        private bool IsValidHex(string value)
        {
            var v = CleanHexPrefix(value);
            
            return Regex.IsMatch(v, "^[0-9a-fA-F]+$");
        }

        public static string CleanHexPrefix(string input)
        {
            return ContainsHexPrefix(input) ? input.Substring(2) : input;
        }

        public static bool ContainsHexPrefix(string input)
        {
            return input.Length> 1 && input[0] == '0' && input[1] == 'x';
        }
        
        /// <summary>
        ///    Adds the pad bytes to the passed in block, returning the number of bytes added 
        /// </summary>
        private static byte[] ToBytesPadded(BigInteger value, int length)
        {
            var result = new byte[length];
            var bytes = value.ToByteArray();

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

            var destOffset = length - bytesLength;
            
            Array.Copy(bytes, srcOffset, result, destOffset, bytesLength);
            
            return result;
        }
    }
}