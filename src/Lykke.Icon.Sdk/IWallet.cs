using Lykke.Icon.Sdk.Data;

namespace Lykke.Icon.Sdk
{
    /**
     * Wallet class signs the message(a transaction message to send)
     * using own key-pair
     */
    public interface IWallet
    {
        /**
         * Gets the address corresponding the key of the wallet
         *
         * @return address
         */
        Address GetAddress();

        /**
         * Signs the data to generate a signature
         *
         * @param data to sign
         * @return signature
         */
        byte[] Sign(byte[] data);
    }
}
