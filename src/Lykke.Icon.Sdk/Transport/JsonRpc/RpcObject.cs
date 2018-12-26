using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
    public class RpcObject : RpcItem
    {
        private readonly Dictionary<string, RpcItem> _items;

        private RpcObject(Dictionary<string, RpcItem> items)
        {
            _items = items;
        }

        public IEnumerable<string> GetKeys()
        {
            return _items.Keys;
        }

        public RpcItem GetItem(string key)
        {
            _items.TryGetValue(key, out var result);
            return result;
        }

        public override string ToString()
        {
            return "RpcObject(items=" + _items + ')';
        }

        public override bool IsEmpty()
        {
            return _items == null || !_items.Any();
        }

        [PublicAPI]
        public class Builder
        {
            private Dictionary<string, RpcItem> _items;

            public Builder() : this(Sort.None)
            {
                
            }

            public Builder(Sort sort)
            {
                switch (sort)
                {
                    //case KEY:
                    //    items = new TreeMap<>();
                    //    break;
                    //case INSERT:
                    //    items = new LinkedHashMap<>();
                    //    break;
                    default:
                        _items = new Dictionary<string, RpcItem>();
                        break;
                }
            }

            public Builder Put(string key, RpcItem item)
            {
                if (!_items.ContainsKey(key) && !IsNullOrEmpty(item))
                {
                    _items[key] = item;
                }
                
                return this;
            }

            public RpcObject Build()
            {
                return new RpcObject(_items);
            }

            private static bool IsNullOrEmpty(RpcItem item)
            {
                return item == null || item.IsEmpty();
            }
            
            public enum Sort
            {
                None,
                Key,
                Insert
            }
        }
    }
}
