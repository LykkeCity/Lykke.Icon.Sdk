using Lykke.Icon.Sdk.Transport.JsonRpc;
using System.Numerics;
using System;
using System.Collections.Generic;

namespace Lykke.Icon.Sdk.Data
{
    public class Block
    {
        private RpcObject properties;

        public Block(RpcObject properties)
        {
            this.properties = properties;
        }

        public RpcObject GetProperties()
        {
            return properties;
        }

        public Bytes GetPrevBlockHash()
        {
            RpcItem item = properties.GetItem("prev_block_hash");
            return item != null ? item.ToBytes() : null;
        }

        public Bytes GetMerkleTreeRootHash()
        {
            RpcItem item = properties.GetItem("merkle_tree_root_hash");
            return item != null ? item.ToBytes() : null;
        }

        public BigInteger GetTimestamp()
        {
            RpcItem item = properties.GetItem("time_stamp");
            return item != null ? item.ToInteger() : 0;
        }

        public List<ConfirmedTransaction> GetTransactions()
        {
            RpcItem item = properties.GetItem("confirmed_transaction_list");
            List<ConfirmedTransaction> transactions = new List<ConfirmedTransaction>();
            if (item != null && GetHeight() > 0)
            {
                foreach (RpcItem tx in item.ToArray())
                {
                    transactions.Add(Converters.CONFIRMED_TRANSACTION.ConvertTo(tx.ToObject()));
                }
            }
            return transactions;
        }

        public Bytes GetBlockHash()
        {
            RpcItem item = properties.GetItem("block_hash");
            return item != null ? item.ToBytes() : null;
        }

        public String GetPeerId()
        {
            RpcItem item = properties.GetItem("peer_id");
            return item != null ? item.ToString() : null;
        }

        public BigInteger GetVersion()
        {
            RpcItem item = properties.GetItem("version");
            return item != null ? item.ToInteger() : 0;
        }

        public BigInteger GetHeight()
        {
            RpcItem item = properties.GetItem("height");
            return item != null ? item.ToInteger() : 0;
        }

        public String GetSignature()
        {
            RpcItem item = properties.GetItem("signature");
            return item != null ? item.ToString() : null;
        }

        public override String ToString()
        {
            return "Block{" +
                    "properties=" + properties +
                    '}';
        }
    }
}
