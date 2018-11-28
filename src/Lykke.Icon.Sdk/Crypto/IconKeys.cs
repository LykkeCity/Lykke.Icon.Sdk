using Lykke.Icon.Sdk.Data;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Crypto.Digests;

namespace Lykke.Icon.Sdk.Crypto
{
    /**
     * Implementation from
     * https://github.com/web3j/web3j/blob/master/crypto/src/main/java/org/web3j/crypto/Keys.java
     * Crypto key utilities.
     */
    public static class IconKeys
    {
        public static int PRIVATE_KEY_SIZE = 32;
        public static int PUBLIC_KEY_SIZE = 64;

        public static int ADDRESS_SIZE = 160;
        public static int ADDRESS_LENGTH_IN_HEX = ADDRESS_SIZE >> 2;
        private static SecureRandom SECURE_RANDOM;

        public static double MIN_BOUNCY_CASTLE_VERSION = 1.46;

        static IconKeys()
        {
            //Provider provider = Security.GetProvider(BouncyCastleProvider.PROVIDER_NAME);
            //Provider newProvider = new BouncyCastleProvider();

            //if (newProvider.getVersion() < MIN_BOUNCY_CASTLE_VERSION)
            //{
            //    String message = String.Format(
            //        "The version of BouncyCastle should be %f or newer", MIN_BOUNCY_CASTLE_VERSION);
            //    throw new Exception(message);
            //}

            //if (provider != null)
            //{
            //    Security.RemoveProvider(BouncyCastleProvider.PROVIDER_NAME);
            //}

            //Security.AddProvider(newProvider);

            SECURE_RANDOM = new SecureRandom();
        }

        public static Bytes CreatePrivateKey()
        {
            IAsymmetricCipherKeyPairGenerator keyPairGenerator = Org.BouncyCastle.Security.GeneratorUtilities.GetKeyPairGenerator("EC");
            var curveParams = Org.BouncyCastle.Crypto.EC.CustomNamedCurves.GetByName("secp256k1");
            var curve = new ECDomainParameters(
                curveParams.Curve, curveParams.G, curveParams.N, curveParams.H);
            ECKeyGenerationParameters ecGenParameterSpec = new ECKeyGenerationParameters(curve, SECURE_RANDOM);
            keyPairGenerator.Init(ecGenParameterSpec);
            var keyPair = keyPairGenerator.GenerateKeyPair();
            return new Bytes(((ECPrivateKeyParameters)keyPair.Private).D);
        }

        public static Bytes GetPublicKey(Bytes privateKey)
        {
            var spec = ECNamedCurveTable.GetByName("secp256k1");
            ECPoint pointQ = spec.G.Multiply(new BigInteger(1, privateKey.ToByteArray()));
            byte[] publicKeyBytes = pointQ.GetEncoded(false);
            return new Bytes(Arrays.CopyOfRange(publicKeyBytes, 1, publicKeyBytes.Length));
        }

        public static Address GetAddress(Bytes publicKey)
        {
            return new Address(new Address.AddressPrefix(Address.AddressPrefix.EOA), GetAddressHash(publicKey.ToByteArray(PUBLIC_KEY_SIZE)));
        }

        public static byte[] GetAddressHash(BigInteger publicKey)
        {
            return GetAddressHash(new Bytes(publicKey).ToByteArray(PUBLIC_KEY_SIZE));
        }

        public static byte[] GetAddressHash(byte[] publicKey)
        {
            byte[] hash = new byte[publicKey.Length];
            Array.Copy(publicKey, hash, publicKey.Length);
            new Sha3Digest(256).DoFinal(hash, 0);

            int length = 20;
            byte[] result = new byte[20];
            Array.Copy(hash, hash.Length - 20, result, 0, length);
            return result;
        }

        public static bool IsValidAddress(Address input)
        {
            return IsValidAddress(input.ToString());
        }

        public static bool IsValidAddress(String input)
        {
            String cleanInput = CleanHexPrefix(input);
            try
            {
                return Regex.IsMatch(cleanInput, "^[0-9a-f]{40}$") && cleanInput.Length == ADDRESS_LENGTH_IN_HEX;
            }
            catch (Exception e)
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
            return address?.GetPrefix().GetValue() == Address.AddressPrefix.CONTRACT;
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
            return Address.AddressPrefix.FromString(input.Substring(0, 2));
        }

        public static SecureRandom SecureRandom()
        {
            return SECURE_RANDOM;
        }
    }
}
