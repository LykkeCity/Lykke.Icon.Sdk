using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Icon.Sdk.Crypto;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Crypto
{
    /**
     * Original Code
     * https://github.com/web3j/web3j/blob/master/crypto/src/main/java/org/web3j/crypto/Sign.java
     * <p>Transaction signing logic.</p>
     *
     * <p>Adapted from the
     * <a href="https://github.com/bitcoinj/bitcoinj/blob/master/core/src/main/java/org/bitcoinj/core/ECKey.java">
     * BitcoinJ ECKey</a> implementation.
     */
    public class ECDSASignature
    {

        private X9ECParameters curveParams = CustomNamedCurves.getByName("secp256k1");
        private ECDomainParameters curve = new ECDomainParameters(
                curveParams.getCurve(), curveParams.getG(), curveParams.getN(), curveParams.getH());

        private Bytes privateKey;

        public ECDSASignature(Bytes privateKey)
        {
            this.privateKey = privateKey;
        }

        /**
         * Serialize the result of `generateSignature` and recovery id
         *
         * @param sig the R and S components of the signature, wrapped.
         * @param message Hash of the data that was signed.
         * @return 32 bytes for R + 32 bytes for S + 1 byte for recovery id
         */
        public byte[] RecoverableSerialize(BigInteger[] sig, byte[] message)
        {
            byte recId = FindRecoveryId(sig, message);

            ByteBuffer buffer = ByteBuffer.Allocate(32 + 32 + 1);
            buffer.put(new Bytes(sig[0]).ToByteArray(32));
            buffer.put(new Bytes(sig[1]).ToByteArray(32));
            buffer.put(recId);
            return buffer.array();
        }

        /**
         * generate a signature for the given message using the key we were initialised with
         *
         * @param message Hash of the data that was signed.
         * @return the R and S components of the signature, wrapped.
         */
        public BigInteger[] GenerateSignature(byte[] message)
        {
            BigInteger p = new BigInteger(1, privateKey.toByteArray());
            ECDSASigner signer = new ECDSASigner(new HMacDSAKCalculator(new SHA256Digest()));
            ECPrivateKeyParameters param = new ECPrivateKeyParameters(p, curve);
            signer.init(true, param);
            BigInteger[] sig = signer.GenerateSignature(message);
            BigInteger r = sig[0];
            BigInteger s = sig[1];
            if (s.CompareTo(curveParams.getN().ShiftRight(1)) > 0)
            {
                s = curve.GetN().Subtract(s);
            }
            return new BigInteger[] { r, s };
        }

        /**
         * Returns the recovery ID, a byte with value between 0 and 3, inclusive, that specifies which of 4 possible
         * curve points was used to sign a message. This value is also referred to as "v".
         *
         * @throws RuntimeException if no recovery ID can be found.
         * @param sig     the R and S components of the signature, wrapped.
         * @param message Hash of the data that was signed.
         * @return recId   Which possible key to recover.
         */
        public byte FindRecoveryId(BigInteger[] sig, byte[] message)
        {
            Bytes publicKey = IconKeys.GetPublicKey(privateKey);
            BigInteger p = new BigInteger(1, publicKey.ToByteArray());
            byte recId = -1;
            for (byte i = 0; i < 4; i++)
            {
                BigInteger k = RecoverFromSignature(i, sig, message);
                if (k != null && k.equals(p))
                {
                    recId = i;
                    break;
                }
            }
            if (recId == -1)
            {
                throw new RuntimeException(
                        "Could not construct a recoverable key. This should never happen.");
            }
            return recId;
        }

        /**
         * <p>Given the components of a signature and a selector value, recover and return the public
         * key that generated the signature according to the algorithm in SEC1v2 section 4.1.6.</p>
         *
         * <p>The recId is an index from 0 to 3 which indicates which of the 4 possible keys is the
         * correct one. Because the key recovery operation yields multiple potential keys, the correct
         * key must either be stored alongside the
         * signature, or you must be willing to try each recId in turn until you find one that outputs
         * the key you are expecting.</p>
         *
         * <p>If this method returns null it means recovery was not possible and recId should be
         * iterated.</p>
         *
         * <p>Given the above two points, a correct usage of this method is inside a for loop from
         * 0 to 3, and if the output is null OR a key that is not the one you expect, you try again
         * with the next recId.</p>
         *
         * @param recId   Which possible key to recover.
         * @param sig     the R and S components of the signature, wrapped.
         * @param message Hash of the data that was signed.
         * @return An ECKey containing only the public part, or null if recovery wasn't possible.
         */
        private BigInteger RecoverFromSignature(int recId, BigInteger[] sig, byte[] message)
        {
            BigInteger r = sig[0];
            BigInteger s = sig[1];

            checkArgument(recId >= 0, "recId must be positive");
            checkArgument(r.Signum() >= 0, "r must be positive");
            checkArgument(s.Signum() >= 0, "s must be positive");
            checkArgument(message != null, "message cannot be null");

            // 1.0 For j from 0 to h   (h == recId here and the loop is outside this function)
            //   1.1 Let x = r + jn
            BigInteger n = curve.GetN();  // Curve order.
            BigInteger i = BigInteger.ValueOf((long)recId / 2);
            BigInteger x = r.Add(i.Multiply(n));
            //   1.2. Convert the integer x to an octet string X of length mlen using the conversion
            //        routine specified in Section 2.3.7, where mlen = ⌈(log2 p)/8⌉ or mlen = ⌈m/8⌉.
            //   1.3. Convert the octet string (16 set binary digits)||X to an elliptic curve point R
            //        using the conversion routine specified in Section 2.3.4. If this conversion
            //        routine outputs "invalid", then do another iteration of Step 1.
            //
            // More concisely, what these points mean is to use X as a compressed public key.
            BigInteger prime = SecP256K1Curve.Q;
            if (x.CompareTo(prime) >= 0)
            {
                // Cannot have point co-ordinates larger than this as everything takes place modulo Q.
                return null;
            }
            // Compressed keys require you to know an extra bit of data about the y-coord as there are
            // two possibilities. So it's encoded in the recId.
            ECPoint ecPoint = decompressKey(x, (recId & 1) == 1);
            //   1.4. If nR != point at infinity, then do another iteration of Step 1 (callers
            //        responsibility).
            if (!ecPoint.multiply(n).IsInfinity())
            {
                return null;
            }
            //   1.5. Compute e from M using Steps 2 and 3 of ECDSASignature signature verification.
            BigInteger e = new BigInteger(1, message);
            //   1.6. For k from 1 to 2 do the following.   (loop is outside this function via
            //        iterating recId)
            //   1.6.1. Compute a candidate public key as:
            //               Q = mi(r) * (sR - eG)
            //
            // Where mi(x) is the modular multiplicative inverse. We transform this into the following:
            //               Q = (mi(r) * s ** R) + (mi(r) * -e ** G)
            // Where -e is the modular additive inverse of e, that is z such that z + e = 0 (mod n).
            // In the above equation ** is point multiplication and + is point addition (the EC group
            // operator).
            //
            // We can find the additive inverse by subtracting e from zero then taking the mod. For
            // example the additive inverse of 3 modulo 11 is 8 because 3 + 8 mod 11 = 0, and
            // -3 mod 11 = 8.
            BigInteger eInv = BigInteger.ZERO.Subtract(e).mod(n);
            BigInteger rInv = r.ModInverse(n);
            BigInteger srInv = rInv.Multiply(s).Mod(n);
            BigInteger eInvrInv = rInv.Multiply(eInv).Mod(n);
            ECPoint q = ECAlgorithms.SumOfTwoMultiplies(curve.GetG(), eInvrInv, ecPoint, srInv);

            byte[] qBytes = q.GetEncoded(false);
            // We remove the prefix
            return new BigInteger(1, Arrays.CopyOfRange(qBytes, 1, qBytes.Length));
        }

        /**
         * Decompress a compressed public key (x co-ord and low-bit of y-coord).
         */
        private ECPoint DecompressKey(BigInteger xBN, boolean yBit)
        {
            X9IntegerConverter x9 = new X9IntegerConverter();
            byte[] compEnc = x9.integerToBytes(xBN, 1 + x9.GetByteLength(curve.GetCurve()));
            compEnc[0] = (byte)(yBit ? 0x03 : 0x02);
            return curve.GetCurve().DecodePoint(compEnc);
        }

        private void CheckArgument(boolean expression, String message)
        {
            if (!expression)
            {
                throw new ArgumentException(message);
            }
        }
    }
}