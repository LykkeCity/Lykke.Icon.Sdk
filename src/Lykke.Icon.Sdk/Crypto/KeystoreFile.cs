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
//     * https://github.com/web3j/web3j/blob/master/crypto/src/main/java/org/web3j/crypto/WalletFile.java
//     * Icon wallet file.
//     */
//    public class KeystoreFile
//    {
//        private String address;
//        private Crypto crypto;
//        private String id;
//        private String coinType;
//        private int version;

//        public KeystoreFile()
//        {
//        }

//        public String GetAddress()
//        {
//            return address;
//        }

//        public void SetAddress(String address)
//        {
//            this.address = address;
//        }

//        public void SetAddress(Address address)
//        {
//            this.address = address.ToString();
//        }

//        public Crypto GetCrypto()
//        {
//            return crypto;
//        }

//        public void SetCrypto(Crypto crypto)
//        {
//            this.crypto = crypto;
//        }

//        //@JsonSetter("Crypto")  // older wallet files may have this attribute name
//        public void SetCryptoV1(Crypto crypto)
//        {
//            setCrypto(crypto);
//        }

//        public String GetId()
//        {
//            return id;
//        }

//        public void SetId(String id)
//        {
//            this.id = id;
//        }

//        public int GetVersion()
//        {
//            return version;
//        }

//        public void SetVersion(int version)
//        {
//            this.version = version;
//        }

//        public String GetCoinType()
//        {
//            return coinType;
//        }

//        public void SetCoinType(String coinType)
//        {
//            this.coinType = coinType;
//        }

//        public override bool Equals(Object o)
//        {
//            if (this == o)
//            {
//                return true;
//            }
//            if (!(o is KeystoreFile))
//            {
//                return false;
//            }

//            KeystoreFile that = (KeystoreFile)o;

//            if (GetAddress() != null
//                    ? !GetAddress().Equals(that.GetAddress())
//                    : that.GetAddress() != null)
//            {
//                return false;
//            }
//            if (GetCrypto() != null
//                    ? !GetCrypto().Equals(that.GetCrypto())
//                    : that.GetCrypto() != null)
//            {
//                return false;
//            }
//            if (GetId() != null
//                    ? !GetId().Equals(that.GetId())
//                    : that.GetId() != null)
//            {
//                return false;
//            }
//            return version == that.version;
//        }

//        public override int GetHashCode()
//        {
//            int result = GetAddress() != null ? GetAddress().GetHashCode() : 0;
//            result = 31 * result + (GetCrypto() != null ? GetCrypto().GetHashCode() : 0);
//            result = 31 * result + (GetId() != null ? GetId().GetHashCode() : 0);
//            result = 31 * result + version;
//            return result;
//        }

//        public static class Crypto
//        {
//            private String cipher;
//            private String ciphertext;
//            private CipherParams cipherparams;

//            private String kdf;
//            private KdfParams kdfparams;

//            private String mac;

//            public Crypto()
//            {
//            }

//            public String GetCipher()
//            {
//                return cipher;
//            }

//            public void SetCipher(String cipher)
//            {
//                this.cipher = cipher;
//            }

//            public String GetCiphertext()
//            {
//                return ciphertext;
//            }

//            public void SetCiphertext(String ciphertext)
//            {
//                this.ciphertext = ciphertext;
//            }

//            public CipherParams GetCipherparams()
//            {
//                return cipherparams;
//            }

//            public void SetCipherparams(CipherParams cipherparams)
//            {
//                this.cipherparams = cipherparams;
//            }

//            public String GetKdf()
//            {
//                return kdf;
//            }

//            public void SetKdf(String kdf)
//            {
//                this.kdf = kdf;
//            }

//            public KdfParams GetKdfparams()
//            {
//                return kdfparams;
//            }

//            //@JsonTypeInfo(
//            //        use = JsonTypeInfo.Id.NAME,
//            //        include = JsonTypeInfo.As.EXTERNAL_PROPERTY,
//            //        property = "kdf")
//            //@JsonSubTypes({
//            //        @JsonSubTypes.Type(value = Aes128CtrKdfParams.class, name = Keystore.AES_128_CTR),
//            //        @JsonSubTypes.Type(value = ScryptKdfParams.class, name = Keystore.SCRYPT)
//            //})
//            // To support my Ether Keystore keys uncomment this annotation & comment out the above
//            //  @JsonDeserialize(using = KdfParamsDeserialiser.class)
//            // Also add the following to the ObjectMapperFactory
//            // objectMapper.configure(MapperFeature.ACCEPT_CASE_INSENSITIVE_PROPERTIES, true);
//            public void SetKdfparams(KdfParams kdfparams)
//            {
//                this.kdfparams = kdfparams;
//            }

//            public String GetMac()
//            {
//                return mac;
//            }

//            public void SetMac(String mac)
//            {
//                this.mac = mac;
//            }

//            public override bool Equals(Object o)
//            {
//                if (this == o)
//                {
//                    return true;
//                }
//                if (!(o is Crypto))
//                {
//                    return false;
//                }

//                Crypto that = (Crypto)o;

//                if (GetCipher() != null
//                        ? !GetCipher().Equals(that.getCipher())
//                        : that.GetCipher() != null)
//                {
//                    return false;
//                }
//                if (GetCiphertext() != null
//                        ? !GetCiphertext().Equals(that.GetCiphertext())
//                        : that.GetCiphertext() != null)
//                {
//                    return false;
//                }
//                if (GetCipherparams() != null
//                        ? !GetCipherparams().Equals(that.GetCipherparams())
//                        : that.GetCipherparams() != null)
//                {
//                    return false;
//                }
//                if (GetKdf() != null
//                        ? !GetKdf().equals(that.GetKdf())
//                        : that.GetKdf() != null)
//                {
//                    return false;
//                }
//                if (GetKdfparams() != null
//                        ? !GetKdfparams().equals(that.GetKdfparams())
//                        : that.GetKdfparams() != null)
//                {
//                    return false;
//                }
//                return GetMac() != null
//                        ? GetMac().Equals(that.GetMac()) : that.GetMac() == null;
//            }

//            public override int GetHashCode()
//            {
//                int result = GetCipher() != null ? GetCipher().GetHashCode() : 0;
//                result = 31 * result + (GetCiphertext() != null ? GetCiphertext().GetHashCode() : 0);
//                result = 31 * result + (GetCipherparams() != null ? GetCipherparams().GetHashCode() : 0);
//                result = 31 * result + (GetKdf() != null ? GetKdf().GetHashCode() : 0);
//                result = 31 * result + (GetKdfparams() != null ? GetKdfparams().GetHashCode() : 0);
//                result = 31 * result + (GetMac() != null ? GetMac().GetHashCode() : 0);
//                return result;
//            }

//        }

//        public static class CipherParams
//        {
//            private String iv;

//            public CipherParams()
//            {
//            }

//            public String GetIv()
//            {
//                return iv;
//            }

//            public void SetIv(String iv)
//            {
//                this.iv = iv;
//            }

//            public override bool Equals(Object o)
//            {
//                if (this == o)
//                {
//                    return true;
//                }
//                if (!(o is CipherParams))
//                {
//                    return false;
//                }

//                CipherParams that = (CipherParams)o;

//                return GetIv() != null
//                        ? GetIv().Equals(that.GetIv()) : that.GetIv() == null;
//            }

//            public override int GetHashCode()
//            {
//                int result = GetIv() != null ? GetIv().GetHashCode() : 0;
//                return result;
//            }

//        }

//        public interface KdfParams
//        {
//            int GetDklen();

//            String GetSalt();
//        }

//        public static class Aes128CtrKdfParams : KdfParams
//        {
//            private int dklen;
//            private int c;
//            private String prf;
//            private String salt;

//            public Aes128CtrKdfParams()
//            {
//            }

//            public int GetDklen()
//            {
//                return dklen;
//            }

//            public void SetDklen(int dklen)
//            {
//                this.dklen = dklen;
//            }

//            public int GetC()
//            {
//                return c;
//            }

//            public void SetC(int c)
//            {
//                this.c = c;
//            }

//            public String GetPrf()
//            {
//                return prf;
//            }

//            public void SetPrf(String prf)
//            {
//                this.prf = prf;
//            }

//            public String GetSalt()
//            {
//                return salt;
//            }

//            public void SetSalt(String salt)
//            {
//                this.salt = salt;
//            }

//            public override bool Equals(Object o)
//            {
//                if (this == o)
//                {
//                    return true;
//                }
//                if (!(o is Aes128CtrKdfParams))
//                {
//                    return false;
//                }

//                Aes128CtrKdfParams that = (Aes128CtrKdfParams)o;

//                if (dklen != that.dklen)
//                {
//                    return false;
//                }
//                if (c != that.c)
//                {
//                    return false;
//                }
//                if (getPrf() != null
//                        ? !GetPrf().Equals(that.GetPrf())
//                        : that.getPrf() != null)
//                {
//                    return false;
//                }
//                return GetSalt() != null
//                        ? GetSalt().equals(that.GetSalt()) : that.GetSalt() == null;
//            }

//            public override int GetHashCode()
//            {
//                int result = dklen;
//                result = 31 * result + c;
//                result = 31 * result + (GetPrf() != null ? GetPrf().GetHashCode() : 0);
//                result = 31 * result + (GetSalt() != null ? GetSalt().GetHashCode() : 0);
//                return result;
//            }
//        }

//        public static class ScryptKdfParams : KdfParams
//        {
//            private int dklen;
//            private int n;
//            private int p;
//            private int r;
//            private String salt;

//            public ScryptKdfParams()
//            {
//            }

//            public int GetDklen()
//            {
//                return dklen;
//            }

//            public void SetDklen(int dklen)
//            {
//                this.dklen = dklen;
//            }

//            public int GetN()
//            {
//                return n;
//            }

//            public void SetN(int n)
//            {
//                this.n = n;
//            }

//            public int GetP()
//            {
//                return p;
//            }

//            public void SetP(int p)
//            {
//                this.p = p;
//            }

//            public int GetR()
//            {
//                return r;
//            }

//            public void SetR(int r)
//            {
//                this.r = r;
//            }

//            public String GetSalt()
//            {
//                return salt;
//            }

//            public void SetSalt(String salt)
//            {
//                this.salt = salt;
//            }

//            public boolean equals(Object o)
//            {
//                if (this == o)
//                {
//                    return true;
//                }
//                if (!(o is ScryptKdfParams))
//                {
//                    return false;
//                }

//                ScryptKdfParams that = (ScryptKdfParams)o;

//                if (dklen != that.dklen)
//                {
//                    return false;
//                }
//                if (n != that.n)
//                {
//                    return false;
//                }
//                if (p != that.p)
//                {
//                    return false;
//                }
//                if (r != that.r)
//                {
//                    return false;
//                }
//                return GetSalt() != null
//                        ? GetSalt().Equals(that.GetSalt()) : that.GetSalt() == null;
//            }

//            public override int GetHashCode()
//            {
//                int result = dklen;
//                result = 31 * result + n;
//                result = 31 * result + p;
//                result = 31 * result + r;
//                result = 31 * result + (getSalt() != null ? getSalt().hashCode() : 0);
//                return result;
//            }
//        }
//    }

//}
