using Lykke.Icon.Sdk.Crypto;
using Lykke.Icon.Sdk.Data;
using Org.BouncyCastle.Math;

namespace Lykke.Icon.Sdk
{
    /**
     * An implementation of Wallet which uses of the key pair.
     */
    public class KeyWallet : IWallet
    {
        private Bytes _privateKey;
        private Bytes _publicKey;

        private KeyWallet(Bytes privateKey, Bytes publicKey)
        {
            this._privateKey = privateKey;
            this._publicKey = publicKey;
        }

        /**
         * Loads a key wallet from the private key
         *
         * @param privateKey the private key to load
         * @return KeyWallet
         */
        public static KeyWallet Load(Bytes privateKey)
        {
            Bytes publicKey = IconKeys.GetPublicKey(privateKey);
            return new KeyWallet(privateKey, publicKey);
        }

        /**
         * Creates a new KeyWallet with generating a new key pair.
         *
         * @return new KeyWallet
         */
        public static KeyWallet Create()
        {
            Bytes privateKey = IconKeys.CreatePrivateKey();
            Bytes publicKey = IconKeys.GetPublicKey(privateKey);

            return new KeyWallet(privateKey, publicKey);
        }

        /**
         * Loads a key wallet from the KeyStore file
         *
         * @param password the password of KeyStore
         * @param file     the KeyStore file
         * @return KeyWallet
         */
        //public static KeyWallet Load(String password, File file)
        //{
        //    Bytes privateKey = KeyStoreUtils.LoadPrivateKey(password, file);
        //    Bytes pubicKey = IconKeys.GetPublicKey(privateKey);
        //    return new KeyWallet(privateKey, pubicKey);
        //}

        /**
         * Stores the KeyWallet as a KeyStore
         *
         * @param wallet               the wallet to store
         * @param password             the password of KeyStore
         * @param destinationDirectory the KeyStore file is stored at.
         * @return name of the KeyStore file
         */
        //public static String Store(KeyWallet wallet, String password, File destinationDirectory)
        //{
        //    KeystoreFile keystoreFile = Keystore.Create(password, wallet.getPrivateKey(), 1 << 14, 1);
        //    return KeyStoreUtils.GenerateWalletFile(keystoreFile, destinationDirectory);
        //}

        /**
         * @see Wallet#getAddress()
         */
        public Address GetAddress()
        {
            return IconKeys.GetAddress(_publicKey);
        }

        /**
         * @see Wallet#sign(byte[])
         */
        public byte[] Sign(byte[] data)
        {
            TransactionBuilder.CheckArgument(data, "hash not found");
            ECDSASignature signature = new ECDSASignature(_privateKey);
            BigInteger[] sig = signature.GenerateSignature(data);
            return signature.RecoverableSerialize(sig, data);
        }

        /**
         * Gets the private key
         *
         * @return private key
         */
        public Bytes GetPrivateKey()
        {
            return _privateKey;
        }

        public Bytes GetPublicKey()
        {
            return _publicKey;
        }

    }
}
