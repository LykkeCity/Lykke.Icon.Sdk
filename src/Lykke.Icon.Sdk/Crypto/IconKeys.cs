using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Icon.Sdk.Crypto;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Crypto
{
    /**
     * Implementation from
     * https://github.com/web3j/web3j/blob/master/crypto/src/main/java/org/web3j/crypto/Keys.java
     * Crypto key utilities.
     */
    public class IconKeys
    {

        public static int PRIVATE_KEY_SIZE = 32;
        public static int PUBLIC_KEY_SIZE = 64;

        public static int ADDRESS_SIZE = 160;
        public static int ADDRESS_LENGTH_IN_HEX = ADDRESS_SIZE >> 2;
        private static SecureRandom SECURE_RANDOM;

        public static double MIN_BOUNCY_CASTLE_VERSION = 1.46;

        static IconKeys()
        {
            Provider provider = Security.GetProvider(BouncyCastleProvider.PROVIDER_NAME);
            Provider newProvider = new BouncyCastleProvider();

            if (newProvider.getVersion() < MIN_BOUNCY_CASTLE_VERSION)
            {
                String message = String.format(
                        "The version of BouncyCastle should be %f or newer", MIN_BOUNCY_CASTLE_VERSION);
                throw new RuntimeCryptoException(message);
            }

            if (provider != null)
            {
                Security.RemoveProvider(BouncyCastleProvider.PROVIDER_NAME);
            }

            Security.AddProvider(newProvider);

            SECURE_RANDOM = new SecureRandom();
        }

        private IconKeys() { }

        public static Bytes CreatePrivateKey()
        {
            KeyPairGenerator keyPairGenerator = KeyPairGenerator.GetInstance("EC", "BC");
            ECGenParameterSpec ecGenParameterSpec = new ECGenParameterSpec("secp256k1");
            keyPairGenerator.initialize(ecGenParameterSpec, SecureRandom());
            KeyPair keyPair = keyPairGenerator.GenerateKeyPair();
            return new Bytes(((BCECPrivateKey)keyPair.GetPrivate()).GetD());
        }

        public static Bytes GetPublicKey(Bytes privateKey)
        {
            ECNamedCurveParameterSpec spec = ECNamedCurveTable.getParameterSpec("secp256k1");
            ECPoint pointQ = spec.GetG().Multiply(new BigInteger(1, privateKey.ToByteArray()));
            byte[] publicKeyBytes = pointQ.GetEncoded(false);
            return new Bytes(Arrays.CopyOfRange(publicKeyBytes, 1, publicKeyBytes.Length));
        }

        public static Address GetAddress(Bytes publicKey)
        {
            return new Address(Address.AddressPrefix.EOA, GetAddressHash(publicKey.ToByteArray(PUBLIC_KEY_SIZE)));
        }

        public static byte[] GetAddressHash(BigInteger publicKey)
        {
            return getAddressHash(new Bytes(publicKey).toByteArray(PUBLIC_KEY_SIZE));
        }

        public static byte[] GetAddressHash(byte[] publicKey)
        {
            byte[] hash = new SHA3.Digest256().digest(publicKey);

            int length = 20;
            byte[] result = new byte[20];
            System.Arraycopy(hash, hash.Length - 20, result, 0, length);
            return result;
        }

        public static boolean IsValidAddress(Address input)
        {
            return IsValidAddress(input.ToString());
        }

        public static bool IsValidAddress(String input)
        {
            String cleanInput = CleanHexPrefix(input);
            try
            {
                return cleanInput.Matches("^[0-9a-f]{40}$") && cleanInput.Length == ADDRESS_LENGTH_IN_HEX;
            }
            catch (NumberFormatException e)
            {
                return false;
            }
        }

        public static bool IsValidAddressBody(byte[] body)
        {
            return body.Length == 20 &&
                    IconKeys.IsValidAddress(Hex.ToHexString(body));
        }

        public static bool IsContractAddress(Address address)
        {
            return address.GetPrefix() == Address.AddressPrefix.CONTRACT;
        }

        public static String CleanHexPrefix(String input)
        {
            if (ContainsHexPrefix(input))
            {
                return input.Substring(2);
            }
            else
            {
                return input;
            }
        }

        public static bool ContainsHexPrefix(String input)
        {
            return GetAddressHexPrefix(input) != null;
        }

        public static Address.AddressPrefix GetAddressHexPrefix(String input)
        {
            return Address.AddressPrefix.FromString(input.substring(0, 2));
        }

        public static SecureRandom secureRandom()
        {
            return SECURE_RANDOM;
        }
    }
