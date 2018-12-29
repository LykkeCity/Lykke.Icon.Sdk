using System.Collections.Generic;
using System.Numerics;
using JetBrains.Annotations;
using Lykke.Icon.Sdk.Transport.JsonRpc;

namespace Lykke.Icon.Sdk.Data
{
    [PublicAPI]
    public class Block
    {
        private readonly RpcObject _properties;

        public Block(RpcObject properties)
        {
            _properties = properties;
        }

        public RpcObject GetProperties()
        {
            return _properties;
        }

        public Bytes GetPrevBlockHash()
        {
            var item = _properties.GetItem("prev_block_hash");
            
            return item?.ToBytes();
        }

        public Bytes GetMerkleTreeRootHash()
        {
            var item = _properties.GetItem("merkle_tree_root_hash");
            
            return item?.ToBytes();
        }

        public BigInteger GetTimestamp()
        {
            var item = _properties.GetItem("time_stamp");
            
            return item?.ToInteger() ?? 0;
        }

        public List<ConfirmedTransaction> GetTransactions()
        {
            var item = _properties.GetItem("confirmed_transaction_list");
            var transactions = new List<ConfirmedTransaction>();
            
            if (item != null && GetHeight() > 0)
            {
                foreach (var tx in item.ToArray())
                {
                    transactions.Add(Converters.ConfirmedTransaction.ConvertTo(tx.ToObject()));
                }
            }
            
            return transactions;
        }

        public Bytes GetBlockHash()
        {
            var item = _properties.GetItem("block_hash");
            
            return item?.ToBytes();
        }

        public string GetPeerId()
        {
            var item = _properties.GetItem("peer_id");
            
            return item?.ToString();
        }

        public BigInteger GetVersion()
        {
            var item = _properties.GetItem("version");
            
            return item?.ToInteger() ?? 0;
        }

        public BigInteger GetHeight()
        {
            var item = _properties.GetItem("height");
            
            return item?.ToInteger() ?? 0;
        }

        public string GetSignature()
        {
            var item = _properties.GetItem("signature");
            
            return item?.ToString();
        }

        public override string ToString()
        {
            return "Block{properties=" + _properties + '}';
        }
    }
}
