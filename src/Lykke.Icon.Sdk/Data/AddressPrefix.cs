using System;
using System.Collections.Generic;

namespace Lykke.Icon.Sdk.Data
{
    public class AddressPrefix
    {
        public const string Eoa = "hx";
        public const string Contract = "cx";
        
        private static readonly IEnumerable<string> PossiblePrefixes = new[] { Eoa, Contract };
        
        private readonly string _prefix;
        
        
        public AddressPrefix(string prefix)
        {
            _prefix = prefix;
        }

        public string GetValue()
        {
            return _prefix;
        }

        public override string ToString()
        {
            return _prefix;
        }

        public static bool operator ==(AddressPrefix a, AddressPrefix b)
        {
            return a?.Equals(b) ?? ReferenceEquals(b, null);
        }

        public static bool operator !=(AddressPrefix a, AddressPrefix b)
        {
            if (ReferenceEquals(a, null))
            {
                return !ReferenceEquals(b, null);
            }

            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this))
            {
                return true;
            }
            
            if (obj is AddressPrefix other)
            {
                return other._prefix == _prefix;
            }
            
            return false;
        }

        public override int GetHashCode()
        {
            return _prefix.GetHashCode();
        }

        public static AddressPrefix FromString(string prefix)
        {
            if (prefix != null)
            {
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var p in PossiblePrefixes)
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