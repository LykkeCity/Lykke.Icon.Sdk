using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Lykke.Icon.Sdk.Data;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Crypto
{
    [PublicAPI]
    public static class IconKeys
    {
        private static SecureRandom _secureRandom;
        
        public static int AddressSize = 160;
        public static int AddressLengthInHex = AddressSize >> 2;
        public static double MinBouncyCastleVersion = 1.46;
        public static int PrivateKeySize = 32;
        public static int PublicKeySize = 64;

        static IconKeys()
        {
            _secureRandom = new SecureRandom();
        }

        public static string CleanHexPrefix(string input)
        {
            return ContainsHexPrefix(input) ? input.Substring(2) : input;
        }

        public static bool ContainsHexPrefix(string input)
        {
            return GetAddressHexPrefix(input) != null;
        }

        public static Bytes CreatePrivateKey()
        {
            var keyPairGenerator = GeneratorUtilities.GetKeyPairGenerator("EC");
            var curveParams = CustomNamedCurves.GetByName("secp256k1");
            var curve = new ECDomainParameters(curveParams.Curve, curveParams.G, curveParams.N, curveParams.H);
            var ecGenParameterSpec = new ECKeyGenerationParameters(curve, _secureRandom);
            
            keyPairGenerator.Init(ecGenParameterSpec);
            
            var keyPair = keyPairGenerator.GenerateKeyPair();
            
            return new Bytes(((ECPrivateKeyParameters)keyPair.Private).D);
        }

        public static Bytes GetPublicKey(Bytes privateKey)
        {
            var pkBytes = privateKey.ToByteArray();
            var spec = ECNamedCurveTable.GetByName("secp256k1");
            var pointQ = spec.G.Multiply(new BigInteger(1, pkBytes));
            var publicKeyBytes = pointQ.GetEncoded(false);
            
            return new Bytes(Arrays.CopyOfRange(publicKeyBytes, 1, publicKeyBytes.Length));
        }

        public static Address GetAddress(Bytes publicKey)
        {
            var prefix = new AddressPrefix(AddressPrefix.Eoa);
            var pkBytes = publicKey.ToByteArray(PublicKeySize);
            var hash = GetAddressHash(pkBytes);
            
            return new Address(prefix, hash);
        }

        public static byte[] GetAddressHash(BigInteger publicKey)
        {
            return GetAddressHash(new Bytes(publicKey).ToByteArray(PublicKeySize));
        }

        public static AddressPrefix GetAddressHexPrefix(string input)
        {
            return AddressPrefix.FromString(input.Substring(0, 2));
        }

        public static byte[] GetAddressHash(byte[] publicKey)
        {
            var digest = new Sha3Digest(256);
            var output = new byte[digest.GetDigestSize()];
            
            digest.BlockUpdate(publicKey, 0, publicKey.Length);
            digest.DoFinal(output, 0);
            
            var result = new byte[20];
            
            Array.Copy(output, output.Length - 20, result, 0, 20);

            return result;
        }

        public static bool IsContractAddress(Address address)
        {
            return address?.GetPrefix().GetValue() == AddressPrefix.Contract;
        }

        public static bool IsValidAddress(Address input)
        {
            return IsValidAddress(input.ToString());
        }

        public static bool IsValidAddress(string input)
        {
            var cleanInput = CleanHexPrefix(input);
            
            try
            {
                return Regex.IsMatch(cleanInput, "^[0-9a-f]{40}$") && cleanInput.Length == AddressLengthInHex;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsValidAddressBody(byte[] body)
        {
            return body.Length == 20 && IsValidAddress(Hex.ToHexString(body));
        }

        public static SecureRandom SecureRandom()
        {
            return _secureRandom;
        }
    }
}
