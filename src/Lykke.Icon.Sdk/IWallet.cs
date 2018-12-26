using Lykke.Icon.Sdk.Data;

namespace Lykke.Icon.Sdk
{
    /// <summary>
    ///    Wallet class signs the message(a transaction message to send) using own key-pair 
    /// </summary>
    public interface IWallet
    {
        /// <summary>
        ///    Gets the address corresponding the key of the wallet 
        /// </summary>
        Address GetAddress();

        /// <summary>
        ///    Signs the data to generate a signature 
        /// </summary>
        byte[] Sign(byte[] data);
    }
}
