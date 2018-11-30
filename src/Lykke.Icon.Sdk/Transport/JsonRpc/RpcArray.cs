using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{

    /*
     * A read-only data class of RpcArray
     */
    public class RpcArray : RpcItem, IEnumerable<RpcItem>
    {
        private List<RpcItem> items;

        private RpcArray(List<RpcItem> items)
        {
            this.items = items;
        }

        public RpcItem Get(int index)
        {
            return items[index];
        }

        public int Size()
        {
            return items.Count();
        }

        public List<RpcItem> ToList()
        {
            return new List<RpcItem>(items);
        }

        public override bool IsEmpty()
        {
            return items == null || !items.Any();
        }

        public IEnumerator<RpcItem> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public override String ToString()
        {
            return "RpcArray(" +
                    "items=" + items +
                    ')';
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /**
         * Builder for RpcArray
         */
        public class Builder
        {
            private List<RpcItem> items;

            public Builder()
            {
                items = new List<RpcItem>();
            }

            public Builder Add(RpcItem item)
            {
                items.Add(item);
                return this;
            }

            public RpcArray Build()
            {
                return new RpcArray(items);
            }
        }
    }
}
