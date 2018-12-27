using System;
using JetBrains.Annotations;
using Lykke.Icon.Sdk.Crypto;
using Lykke.Icon.Sdk.Data;
using Org.BouncyCastle.Math;

namespace Lykke.Icon.Sdk
{
    /// <inheritdoc />
    [PublicAPI]
    public class KeyWallet : IWallet
    {
        private readonly Bytes _privateKey;
        private readonly Bytes _publicKey;

        private KeyWallet(Bytes privateKey, Bytes publicKey)
        {
            _privateKey = privateKey;
            _publicKey = publicKey;
        }

        /// <summary>
        ///    Loads a key wallet from the private key
        /// </summary>
        public static KeyWallet Load(Bytes privateKey)
        {
            var publicKey = IconKeys.GetPublicKey(privateKey);

            return new KeyWallet(privateKey, publicKey);
        }

        /// <summary>
        ///    Creates a wallet which uses the key pair 
        /// </summary>
        public static KeyWallet Create()
        {
            var privateKey = IconKeys.CreatePrivateKey();
            var publicKey = IconKeys.GetPublicKey(privateKey);

            return new KeyWallet(privateKey, publicKey);
        }

        /// <inheritdoc />
        public Address GetAddress()
        {
            return IconKeys.GetAddress(_publicKey);
        }

        /// <summary>
        ///    Gets the private key 
        /// </summary>
        public Bytes GetPrivateKey()
        {
            return _privateKey;
        }

        /// <summary>
        ///    Gets the public key 
        /// </summary>
        public Bytes GetPublicKey()
        {
            return _publicKey;
        }

        /// <inheritdoc />
        public byte[] Sign(byte[] data)
        {
            TransactionBuilder.CheckArgument(data, "hash not found");

            var signature = new EcdsaSignature(_privateKey);
            var sig = signature.GenerateSignature(data);

            return signature.RecoverableSerialize(sig, data);
        }
    }
}
