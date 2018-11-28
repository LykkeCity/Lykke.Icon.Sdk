//using System;
//using System.Collections.Generic;
//using System.Text;
//using Lykke.Icon.Sdk.Crypto;
//using Org.BouncyCastle.Utilities;
//using Org.BouncyCastle.Utilities.Encoders;

//namespace Lykke.Icon.Sdk.Crypto
//{

//    /**
//     * Original Code
//     * https://github.com/web3j/web3j/blob/master/crypto/src/main/java/org/web3j/crypto/Wallet.java
//     *
//     * <p>Ethereum wallet file management. For reference, refer to
//     * <a href="https://github.com/ethereum/wiki/wiki/Web3-Secret-Storage-Definition">
//     * Web3 Secret Storage Definition</a> or the
//     * <a href="https://github.com/ethereum/go-ethereum/blob/master/accounts/key_store_passphrase.go">
//     * Go Ethereum client implementation</a>.</p>
//     *
//     * <p><strong>Note:</strong> the Bouncy Castle Scrypt implementation
//     * {@link SCrypt}, fails to comply with the following
//     * Ethereum reference
//     * <a href="https://github.com/ethereum/wiki/wiki/Web3-Secret-Storage-Definition#scrypt">
//     * Scrypt test vector</a>:</p>
//     *
//     * <pre>
//     * {@code
//     * // Only value of r that cost (as an int) could be exceeded for is 1
//     * if (r == 1 && N_STANDARD > 65536)
//     * {
//     *     throw new IllegalArgumentException("Cost parameter N_STANDARD must be > 1 and < 65536.");
//     * }
//     * }
//     * </pre>
//     */
//    public class Keystore
//    {

//        private static final int N_LIGHT = 1 << 12;
//        private static final int P_LIGHT = 6;

//        private static final int N_STANDARD = 1 << 18;
//        private static final int P_STANDARD = 1;

//        private static final int R = 8;
//        private static final int DKLEN = 32;

//        private static final int CURRENT_VERSION = 3;

//        private static final String CIPHER = "aes-128-ctr";
//    static final String AES_128_CTR = "pbkdf2";
//    static final String SCRYPT = "scrypt";

//    public static KeystoreFile Create(String password, Bytes privateKey, int n, int p)
//        {

//            byte[] salt = GenerateRandomBytes(32);

//            byte[] derivedKey = GenerateDerivedScryptKey(
//                    password.GetBytes(Charset.ForName("UTF-8")), salt, n, R, p, DKLEN);

//            byte[] encryptKey = Arrays.CopyOfRange(derivedKey, 0, 16);
//            byte[] iv = GenerateRandomBytes(16);


//            byte[] privateKeyBytes = privateKey.ToByteArray(IconKeys.PRIVATE_KEY_SIZE);


//            byte[] cipherText = PerformCipherOperation(
//                    Cipher.ENCRYPT_MODE, iv, encryptKey, privateKeyBytes);

//            byte[] mac = GenerateMac(derivedKey, cipherText);

//            return CreateWalletFile(privateKey, cipherText, iv, salt, mac, n, p);
//        }


//        private static KeystoreFile CreateWalletFile(
//                Bytes privateKey, byte[] cipherText, byte[] iv, byte[] salt, byte[] mac,
//                int n, int p)
//        {

//            KeystoreFile keystoreFile = new KeystoreFile();
//            keystoreFile.SetAddress(IconKeys.GetAddress(IconKeys.GetPublicKey(privateKey)));

//            KeystoreFile.Crypto crypto = new KeystoreFile.Crypto();
//            crypto.SetCipher(CIPHER);
//            crypto.SetCiphertext(Hex.ToHexString(cipherText));
//            keystoreFile.SetCrypto(crypto);

//            KeystoreFile.CipherParams cipherParams = new KeystoreFile.CipherParams();
//            cipherParams.SetIv(Hex.ToHexString(iv));
//            crypto.SetCipherparams(cipherParams);

//            crypto.SetKdf(SCRYPT);
//            KeystoreFile.ScryptKdfParams kdfParams = new KeystoreFile.ScryptKdfParams();
//            kdfParams.SetDklen(DKLEN);
//            kdfParams.SetN(n);
//            kdfParams.SetP(p);
//            kdfParams.SetR(R);
//            kdfParams.SetSalt(Hex.toHexString(salt));
//            crypto.SetKdfparams(kdfParams);

//            crypto.SetMac(Hex.toHexString(mac));
//            keystoreFile.SetCrypto(crypto);
//            keystoreFile.SetId(UUID.randomUUID().toString());
//            keystoreFile.SetVersion(CURRENT_VERSION);
//            keystoreFile.SetCoinType("icx");
//            return keystoreFile;
//        }

//        private static byte[] GgenerateDerivedScryptKey(
//                byte[] password, byte[] salt, int n, int r, int p, int dkLen)
//        {
//            return SCrypt.Generate(password, salt, n, r, p, dkLen);
//        }

//        private static byte[] GenerateAes128CtrDerivedKey(
//                byte[] password, byte[] salt, int c, String prf)
//        {

//            if (!prf.Equals("hmac-sha256"))
//            {
//                throw new KeystoreException("Unsupported prf:" + prf);
//            }

//            // Java 8 supports this, but you have to convert the password to a character array, see
//            // http://stackoverflow.com/a/27928435/3211687

//            PKCS5S2ParametersGenerator gen = new PKCS5S2ParametersGenerator(new SHA256Digest());
//            gen.Init(password, salt, c);
//            return ((KeyParameter)gen.GenerateDerivedParameters(256)).GetKey();
//        }

//        private static byte[] performCipherOperation(
//                int mode, byte[] iv, byte[] encryptKey, byte[] text)
//        {

//            try
//            {
//                IvParameterSpec ivParameterSpec = new IvParameterSpec(iv);
//                Cipher cipher = Cipher.GetInstance("AES/CTR/NoPadding");

//                SecretKeySpec secretKeySpec = new SecretKeySpec(encryptKey, "AES");
//                cipher.Init(mode, secretKeySpec, ivParameterSpec);
//                return cipher.DoFinal(text);
//            }
//            catch (Exception e)
//            {
//                throw new KeystoreException("Error performing cipher operation", e);
//            }
//        }

//        private static byte[] GenerateMac(byte[] derivedKey, byte[] cipherText)
//        {
//            byte[] result = new byte[16 + cipherText.length];

//            System.Arraycopy(derivedKey, 16, result, 0, 16);
//            System.Arraycopy(cipherText, 0, result, 16, cipherText.length);

//            Keccak.DigestKeccak kecc = new Keccak.Digest256();
//            kecc.Update(result, 0, result.length);
//            return kecc.Digest();
//        }

//        public static Bytes decrypt(String password, KeystoreFile keystoreFile)
//        {

//            Validate(keystoreFile);

//            KeystoreFile.Crypto crypto = keystoreFile.GetCrypto();

//            byte[] mac = Hex.Decode(crypto.GetMac());
//            byte[] iv = Hex.Decode(crypto.GetCipherparams().GetIv());
//            byte[] cipherText = Hex.Decode(crypto.GetCiphertext());

//            byte[] derivedKey;

//            KeystoreFile.KdfParams kdfParams = crypto.GetKdfparams();
//            if (kdfParams is KeystoreFile.ScryptKdfParams)
//            {
//                KeystoreFile.ScryptKdfParams scryptKdfParams =
//                        (KeystoreFile.ScryptKdfParams)crypto.GetKdfparams();
//                int dklen = scryptKdfParams.GetDklen();
//                int n = scryptKdfParams.GetN();
//                int p = scryptKdfParams.GetP();
//                int r = scryptKdfParams.GetR();
//                byte[] salt = Hex.Decode(scryptKdfParams.GetSalt());
//                derivedKey = GenerateDerivedScryptKey(password.GetBytes(Charset.ForName("UTF-8")), salt, n, r, p, dklen);
//            }
//            else if (kdfParams is KeystoreFile.Aes128CtrKdfParams)
//            {
//                KeystoreFile.Aes128CtrKdfParams aes128CtrKdfParams =
//                        (KeystoreFile.Aes128CtrKdfParams)crypto.GetKdfparams();
//                int c = aes128CtrKdfParams.GetC();
//                String prf = aes128CtrKdfParams.GetPrf();
//                byte[] salt = Hex.Decode(aes128CtrKdfParams.GetSalt());

//                derivedKey = GenerateAes128CtrDerivedKey(password.GetBytes(Charset.ForName("UTF-8")), salt, c, prf);
//            }
//            else
//            {
//                throw new KeystoreException("Unable to deserialize params: " + crypto.GetKdf());
//            }

//            byte[] derivedMac = GenerateMac(derivedKey, cipherText);

//            if (!Arrays.Equals(derivedMac, mac))
//            {
//                throw new KeystoreException("Invalid password provided");
//            }

//            byte[] encryptKey = Arrays.CopyOfRange(derivedKey, 0, 16);
//            byte[] privateKey = PerformCipherOperation(Cipher.DECRYPT_MODE, iv, encryptKey, cipherText);
//            return new Bytes(privateKey);
//        }

//        private static void validate(KeystoreFile keystoreFile)
//        {
//            KeystoreFile.Crypto crypto = keystoreFile.GetCrypto();

//            if (keystoreFile.GetVersion() != CURRENT_VERSION)
//            {
//                throw new KeystoreException("Keystore version is not supported");
//            }

//            if (!crypto.GetCipher().Equals(CIPHER))
//            {
//                throw new KeystoreException("Keystore cipher is not supported");
//            }

//            if (!crypto.GetKdf().Equals(AES_128_CTR) && !crypto.GetKdf().Equals(SCRYPT))
//            {
//                throw new KeystoreException("KDF type is not supported");
//            }

//            if (keystoreFile.GetCoinType() == null || !keystoreFile.GetCoinType().EqualsIgnoreCase("icx"))
//                throw new KeystoreException("Invalid Keystore file");
//        }

//        private static byte[] GenerateRandomBytes(int size)
//        {
//            byte[] bytes = new byte[size];
//            SecureRandom().NextBytes(bytes);
//            return bytes;
//        }
//    }
//}