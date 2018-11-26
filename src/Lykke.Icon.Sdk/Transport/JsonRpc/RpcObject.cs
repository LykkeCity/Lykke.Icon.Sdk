using System;
using System.Collections.Generic;
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

        public RpcItem GetItem(String key)
        {
            return items.get(key);
        }

        public override String ToString()
        {
            return "RpcObject(" +
                   "items=" + items +
                   ')';
        }

        public override bool IsEmpty()
        {
            return items == null || items.IsEmpty();
        }


        /**
         * Builder for RpcObject
         */
        public static class Builder
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

            public Builder()
            {
                this(Sort.NONE);
            }

            public Builder(Sort sort)
            {
                switch (sort)
                {
                    case KEY:
                        items = new TreeMap<>();
                        break;
                    case INSERT:
                        items = new LinkedHashMap<>();
                        break;
                    default:
                        items = new HashMap<>();
                        break;
                }
            }

            public Builder put(String key, RpcItem item)
            {
                if (!items.containsKey(key) && !isNullOrEmpty(item)) items.put(key, item);
                return this;
            }

            public RpcObject build()
            {
                return new RpcObject(items);
            }

            public boolean isNullOrEmpty(RpcItem item)
            {
                return item == null || item.isEmpty();
            }
        }
    }
}
