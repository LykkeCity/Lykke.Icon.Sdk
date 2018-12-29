using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Icon.Sdk.Data;

namespace Lykke.Icon.Sdk
{
    /// <summary>
    ///   IconService which provides APIs of ICON network. 
    /// </summary>
    public interface IIconService
    {
        /// <summary>
        ///    Get the balance of an address 
        /// </summary>
        Task<BigInteger> GetBalance(Address address);
        
        /// <summary>
        ///    Get a block matching the block number
        /// </summary>
        Task<Block> GetBlock(BigInteger height);
        
        /// <summary>
        ///    Get a block matching the block hash
        /// </summary>
        Task<Block> GetBlock(Bytes hash);
        
        /// <summary>
        ///    Calls a SCORE API for reading
        /// </summary>
        Task<T> CallAsync<T>(Call<T> call);
        
        /// <summary>
        ///    Get the latest block
        /// </summary>
        Task<Block> GetLastBlock();
        
        /// <summary>
        ///    Get information about api function in score
        /// </summary>
        Task<List<ScoreApi>> GetScoreApi(Address scoreAddress);
        
        /// <summary>
        ///    Get the total number of issued coins
        /// </summary>
        Task<BigInteger> GetTotalSupply();
        
        /// <summary>
        ///    Get a transaction matching the given transaction hash
        /// </summary>
        Task<ConfirmedTransaction> GetTransaction(Bytes hash);
        
        /// <summary>
        ///    Get the result of a transaction by transaction hash
        /// </summary>
        Task<TransactionResult> GetTransactionResult(Bytes hash);
        
        /// <summary>
        ///    Sends a transaction that changes the states of account
        /// </summary>
        Task<Bytes> SendTransaction(SignedTransaction signedTransaction);
    }
}