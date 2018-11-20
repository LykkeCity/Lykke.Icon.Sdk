using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Icon.Sdk.Crypto;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

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
            AddressPrefix addressPrefix = IconKeys.getAddressHexPrefix(address);
            if (addressPrefix == null)
            {
                throw new ArgumentException("Invalid address prefix");
            }
            else if (!IconKeys.isValidAddress(address))
            {
                throw new ArgumentException("Invalid address");
            }

            this.prefix = addressPrefix;
            this.body = getAddressBody(address);
        }

        public Address(AddressPrefix prefix, byte[] body)
        {
            if (!IconKeys.isValidAddressBody(body))
            {
                throw new IllegalArgumentException("Invalid address");
            }

            this.prefix = prefix;
            this.body = body;
        }

        private byte[] getAddressBody(String address)
        {
            String cleanInput = IconKeys.cleanHexPrefix(address);
            return Hex.decode(cleanInput);
        }

        public AddressPrefix getPrefix()
        {
            return prefix;
        }

        public boolean isMalformed()
        {
            return isMalformed;
        }

    public String toString()
        {
            if (isMalformed)
            {
                return malformedAddress;
            }
            else
            {
                return getPrefix().getValue() + Hex.toHexString(body);
            }
        }



    public boolean equals(Object obj)
        {
            if (obj == this) return true;
            if (obj instanceof Address) {
                Address other = (Address)obj;
                if (isMalformed)
                {
                    return malformedAddress.equals(other.malformedAddress);
                }
                else
                {
                    return !other.isMalformed && other.prefix == prefix && Arrays.equals(other.body, body);
                }
            }
            return false;
        }

        public enum AddressPrefix
        {

            EOA("hx"),
        CONTRACT("cx");

        private String prefix;

        AddressPrefix(String prefix)
        {
            this.prefix = prefix;
        }

        public String getValue()
        {
            return prefix;
        }

        public static AddressPrefix fromString(String prefix)
        {
            if (prefix != null)
            {
                for (AddressPrefix p : AddressPrefix.values())
                {
                    if (prefix.EqualsIgnoreCase(p.getValue()))
                    {
                        return p;
                    }
                }
            }
            return null;
        }
    }

}

}
