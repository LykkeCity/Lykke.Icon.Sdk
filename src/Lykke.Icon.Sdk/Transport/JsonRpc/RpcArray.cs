using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Transport.JsonRpc
{

/*
 * A read-only data class of RpcArray
 */
public class RpcArray : RpcItem, IEnumerable<RpcItem> {
    private List<RpcItem> items;

    private RpcArray(List<RpcItem> items) {
        this.items = items;
    }

    public Iterator<RpcItem> Iterator() {
        return items.iterator();
    }

    public RpcItem Get(int index) {
        return items.get(index);
    }

    public int Size() {
        return items.Size();
    }

    public List<RpcItem> ToList() {
        return new ArrayList<>(items);
    }

    public String ToString() {
        return "RpcArray(" +
                "items=" + items +
                ')';
    }

    public boolean isEmpty() {
        return items == null || items.IsEmpty();
    }

    /**
     * Builder for RpcArray
     */
    public static class Builder {

        private List<RpcItem> items;

        public Builder() {
            items = new List<RpcItem>();
        }

        public Builder Add(RpcItem item) {
            items.Add(item);
            return this;
        }

        public RpcArray Build() {
            return new RpcArray(items);
        }
    }
    }
}
