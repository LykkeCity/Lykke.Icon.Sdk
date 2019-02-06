using System;
using Lykke.Icon.Sdk.Crypto;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Data
{
    public class Address
    {
        private readonly AddressPrefix _prefix;
        private readonly byte[] _body;
        
        private bool _isMalformed;
        private string _malformedAddress;

        private Address()
        {
            
        }

        public Address(string address)
        {
            var addressPrefix = IconKeys.GetAddressHexPrefix(address);
            
            if (addressPrefix == null)
            {
                throw new ArgumentException("Invalid address prefix");
            }
            else if (!IconKeys.IsValidAddress(address))
            {
                throw new ArgumentException("Invalid address");
            }

            _prefix = addressPrefix;
            _body = GetAddressBody(address);
        }

        public Address(AddressPrefix prefix, byte[] body)
        {
            if (!IconKeys.IsValidAddressBody(body))
            {
                throw new ArgumentException("Invalid address");
            }

            _prefix = prefix;
            _body = body;
        }

        public static Address CreateMalformedAddress(string malformedAddress)
        {
            var address = new Address
            {
                _isMalformed = true,
                _malformedAddress = malformedAddress
            };
            
            return address;
        }

        public AddressPrefix GetPrefix()
        {
            return _prefix;
        }

        public bool IsMalformed()
        {
            return _isMalformed;
        }

        public override string ToString()
        {
            if (_isMalformed)
            {
                return _malformedAddress;
            }
            else
            {
                return GetPrefix().GetValue() + Hex.ToHexString(_body);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            
            if (obj is Address other)
            {
                if (_isMalformed)
                {
                    return _malformedAddress.Equals(other._malformedAddress);
                }
                else
                {
                    return !other._isMalformed 
                        && other._prefix == _prefix
                        && Arrays.AreEqual(other._body, _body);
                }
            }
            
            return false;
        }

        public static bool operator ==(Address a, Address b)
        {
            return a?.Equals(b) ?? ReferenceEquals(b, null);
        }

        public static bool operator !=(Address a, Address b)
        {
            if (ReferenceEquals(a, null))
            {
                return !ReferenceEquals(b, null);
            }

            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public static byte[] GetAddressBody(string address)
        {
            var cleanInput = IconKeys.CleanHexPrefix(address);
            
            return Hex.Decode(cleanInput);
        }
    }
}
