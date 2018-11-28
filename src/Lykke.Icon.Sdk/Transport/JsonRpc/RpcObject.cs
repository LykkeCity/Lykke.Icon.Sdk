using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{

/**
 * A read-only data class of RpcObject
 */
    public class RpcObject : RpcItem
    {
        private Dictionary<String, RpcItem> items;

        private RpcObject(Dictionary<String, RpcItem> items)
        {
            this.items = items;
        }

        public IEnumerable<String> GetKeys()
        {
            return items.Keys;
        }

        public RpcItem GetItem(String key)
        {
            return items[key];
        }

        public override String ToString()
        {
            return "RpcObject(" +
                   "items=" + items +
                   ')';
        }

        public override bool IsEmpty()
        {
            return items == null || !items.Any();
        }


        /**
         * Builder for RpcObject
         */
        public class Builder
        {

            /**
             * Sort policy of the properties
             */
            public enum Sort
            {
                NONE,
                KEY,
                INSERT
            }

            private Dictionary<String, RpcItem> items;

            public Builder() : this(Sort.NONE)
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
                        items = new Dictionary<String, RpcItem>();
                        break;
                }
            }

            public Builder Put(String key, RpcItem item)
            {
                if (!items.ContainsKey(key) && !isNullOrEmpty(item))
                    items[key] = item;
                return this;
            }

            public RpcObject build()
            {
                return new RpcObject(items);
            }

            public bool isNullOrEmpty(RpcItem item)
            {
                return item == null || item.IsEmpty();
            }
        }
    }
}
