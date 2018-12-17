using Lykke.Icon.Sdk.Crypto;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;

namespace Lykke.Icon.Sdk.Data
{
    public class Address
    {
        private AddressPrefix prefix;
        private byte[] body;
        private bool isMalformed = false;
        private String malformedAddress;

        public static Address CreateMalformedAddress(String malformedAddress)
        {
            Address address = new Address();
            address.isMalformed = true;
            address.malformedAddress = malformedAddress;
            return address;
        }

        private Address()
        {
        }

        public Address(String address)
        {
            AddressPrefix addressPrefix = IconKeys.GetAddressHexPrefix(address);
            if (addressPrefix == null)
            {
                throw new ArgumentException("Invalid address prefix");
            }
            else if (!IconKeys.IsValidAddress(address))
            {
                throw new ArgumentException("Invalid address");
            }

            this.prefix = addressPrefix;
            this.body = GetAddressBody(address);
        }

        public Address(AddressPrefix prefix, byte[] body)
        {
            if (!IconKeys.IsValidAddressBody(body))
            {
                throw new ArgumentException("Invalid address");
            }

            this.prefix = prefix;
            this.body = body;
        }

        private byte[] GetAddressBody(String address)
        {
            String cleanInput = IconKeys.CleanHexPrefix(address);
            return Hex.Decode(cleanInput);
        }

        public AddressPrefix GetPrefix()
        {
            return prefix;
        }

        public bool IsMalformed()
        {
            return isMalformed;
        }

        public override String ToString()
        {
            if (isMalformed)
            {
                return malformedAddress;
            }
            else
            {
                return GetPrefix().GetValue() + Hex.ToHexString(body);
            }
        }

        public override bool Equals(Object obj)
        {
            if (obj == this) return true;
            if (obj is Address)
            {
                Address other = (Address)obj;
                if (isMalformed)
                {
                    return malformedAddress.Equals(other.malformedAddress);
                }
                else
                {
                    return !other.isMalformed && other.prefix == prefix && Arrays.AreEqual(other.body, body);
                }
            }
            return false;
        }

        public class AddressPrefix
        {
            public const string EOA = "hx";
            public const string CONTRACT = "cx";

            private String prefix;

            public AddressPrefix(String prefix)
            {
                this.prefix = prefix;
            }

            public String GetValue()
            {
                return prefix;
            }

            public override string ToString()
            {
                return prefix;
            }

            public static bool operator ==(AddressPrefix a, AddressPrefix b)
            {
                if (ReferenceEquals(a , null))
                    return ReferenceEquals(b, null);
                return a.Equals(b);
            }

            public static bool operator !=(AddressPrefix a, AddressPrefix b)
            {
                if (ReferenceEquals(a, null))
                    return !ReferenceEquals(b, null);

                return !a.Equals(b);
            }

            public override bool Equals(Object obj)
            {
                if (obj == this) return true;
                if (obj is AddressPrefix)
                {
                    AddressPrefix other = (AddressPrefix)obj;

                    return other.prefix == this.prefix;
                }
                return false;
            }

            private static IEnumerable<string> _possiblePrefixes = new[] { EOA, CONTRACT };
            public static AddressPrefix FromString(String prefix)
            {
                if (prefix != null)
                {
                    foreach (var p in _possiblePrefixes)
                    {
                        if (prefix.Equals(p, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return new AddressPrefix(p);
                        }
                    }
                }
                return null;
            }
        }
    }
}
