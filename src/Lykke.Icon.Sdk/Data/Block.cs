using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Icon.Sdk.Crypto;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Data
{
    public class Block
    {
        private RpcObject properties;

        Block(RpcObject properties)
        {
            this.properties = properties;
        }

        public RpcObject GetProperties()
        {
            return properties;
        }

        public Bytes GetPrevBlockHash()
        {
            RpcItem item = properties.getItem("prev_block_hash");
            return item != null ? item.asBytes() : null;
        }

        public Bytes GetMerkleTreeRootHash()
        {
            RpcItem item = properties.getItem("merkle_tree_root_hash");
            return item != null ? item.asBytes() : null;
        }

        public BigInteger GetTimestamp()
        {
            RpcItem item = properties.getItem("time_stamp");
            return item != null ? item.asInteger() : null;
        }

        public List<ConfirmedTransaction> GetTransactions()
        {
            RpcItem item = properties.getItem("confirmed_transaction_list");
            List<ConfirmedTransaction> transactions = new ArrayList<>();
            if (item != null && GetHeight().IntValue() > 0)
            {
                for (RpcItem tx : item.ToArray())
                {
                    transactions.add(CONFIRMED_TRANSACTION.convertTo(tx.ToObject()));
                }
            }
            return transactions;
        }

        public Bytes GetBlockHash()
        {
            RpcItem item = properties.getItem("block_hash");
            return item != null ? item.asBytes() : null;
        }

        public String GetPeerId()
        {
            RpcItem item = properties.getItem("peer_id");
            return item != null ? item.asString() : null;
        }

        public BigInteger GetVersion()
        {
            RpcItem item = properties.getItem("version");
            return item != null ? item.asInteger() : null;
        }

        public BigInteger GetHeight()
        {
            RpcItem item = properties.getItem("height");
            return item != null ? item.asInteger() : null;
        }

        public String GetSignature()
        {
            RpcItem item = properties.getItem("signature");
            return item != null ? item.asString() : null;
        }

        public String ToString()
        {
            return "Block{" +
                    "properties=" + properties +
                    '}';
        }
    }
}
