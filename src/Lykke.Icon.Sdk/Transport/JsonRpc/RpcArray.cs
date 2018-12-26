using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{
    [PublicAPI]
    public class RpcArray : RpcItem, IEnumerable<RpcItem>
    {
        private readonly List<RpcItem> _items;

        private RpcArray(List<RpcItem> items)
        {
            _items = items;
        }

        public RpcItem Get(int index)
        {
            return _items[index];
        }

        public int Size()
        {
            return _items.Count();
        }

        public List<RpcItem> ToList()
        {
            return new List<RpcItem>(_items);
        }

        public override bool IsEmpty()
        {
            return _items == null || !_items.Any();
        }

        public IEnumerator<RpcItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public override string ToString()
        {
            return "RpcArray(items=" + _items + ')';
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public class Builder
        {
            private readonly List<RpcItem> _items;

            public Builder()
            {
                _items = new List<RpcItem>();
            }

            public Builder Add(RpcItem item)
            {
                _items.Add(item);
                
                return this;
            }

            public RpcArray Build()
            {
                return new RpcArray(_items);
            }
        }
    }
}
