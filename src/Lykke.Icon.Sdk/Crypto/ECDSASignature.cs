using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Icon.Sdk.Data;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Crypto
{
    [PublicAPI]
    public class EcdsaSignature
    {
        private static readonly BigInteger SecP256K1CurveQ 
            = new BigInteger(1, Hex.Decode("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFC2F"));

        
        private readonly X9ECParameters _curveParams;
        private readonly ECDomainParameters _curve;
        private readonly Bytes _privateKey;

        
        public EcdsaSignature(Bytes privateKey)
        {
            _curveParams = CustomNamedCurves.GetByName("secp256k1");
            _curve = new ECDomainParameters(_curveParams.Curve, _curveParams.G, _curveParams.N, _curveParams.H);
            _privateKey = privateKey;
        }

        /// <summary>
        ///    Serializes the result of `generateSignature` and recovery id
        /// </summary>
        public byte[] RecoverableSerialize(BigInteger[] sig, byte[] message)
        {
            var recId = FindRecoveryId(sig, message);

            var buffer = new List<byte>(32 + 32 + 1);
            buffer.AddRange(new Bytes(sig[0]).ToByteArray(32));
            buffer.AddRange(new Bytes(sig[1]).ToByteArray(32));
            buffer.Add(recId);
            return buffer.ToArray();
        }

        /// <summary>
        ///    Generates a signature for the given message using the key we were initialised with
        /// </summary>
        public BigInteger[] GenerateSignature(byte[] message)
        {
            var p = new BigInteger(1, _privateKey.ToByteArray());
            var signer = new ECDsaSigner(new HMacDsaKCalculator(new Sha256Digest()));
            var param = new ECPrivateKeyParameters(p, _curve);
            
            signer.Init(true, param);
            
            var sig = signer.GenerateSignature(message);
            var r = sig[0];
            var s = sig[1];
            
            if (s.CompareTo(_curveParams.N.ShiftRight(1)) > 0)
            {
                s = _curve.N.Subtract(s);
            }
            
            return new[] { r, s };
        }

        /// <summary>
        ///    Returns the recovery ID, a byte with value between 0 and 3, inclusive, that specifies which of 4 possible
        ///    curve points was used to sign a message. This value is also referred to as "v".
        /// </summary>
        public byte FindRecoveryId(BigInteger[] sig, byte[] message)
        {
            var publicKey = IconKeys.GetPublicKey(_privateKey);
            var p = new BigInteger(1, publicKey.ToByteArray());
            var recId = -1;
            
            for (byte i = 0; i < 4; i++)
            {
                var k = RecoverFromSignature(i, sig, message);
                if (k != null && k.Equals(p))
                {
                    recId = i;
                    break;
                }
            }
            
            if (recId == -1)
            {
                throw new Exception("Could not construct a recoverable key. This should never happen.");
            }
            
            return (byte)recId;
        }

        /// <summary>
        ///    <p>Given the components of a signature and a selector value, recover and return the public
        ///    key that generated the signature according to the algorithm in SEC1v2 section 4.1.6.</p>
        ///    
        ///    <p>The recId is an index from 0 to 3 which indicates which of the 4 possible keys is the
        ///    correct one. Because the key recovery operation yields multiple potential keys, the correct
        ///    key must either be stored alongside the
        ///    signature, or you must be willing to try each recId in turn until you find one that outPuts
        ///    the key you are expecting.</p>
        ///    
        ///    <p>If this method returns null it means recovery was not possible and recId should be
        ///    iterated.</p>
        ///    
        ///    <p>Given the above two points, a correct usage of this method is inside a for loop from
        ///    0 to 3, and if the outPut is null OR a key that is not the one you expect, you try again
        ///    with the next recId.</p> 
        /// </summary>
        private BigInteger RecoverFromSignature(int recId, IReadOnlyList<BigInteger> sig, byte[] message)
        {
            var r = sig[0];
            var s = sig[1];

            // TODO: check .Signum()?
            
            CheckArgument(recId >= 0, "recId must be positive");
            CheckArgument(r.SignValue >= 0, "r must be positive");
            CheckArgument(s.SignValue >= 0, "s must be positive");
            CheckArgument(message != null, "message cannot be null");

            // 1.0 For j from 0 to h   (h == recId here and the loop is outside this function)
            //   1.1 Let x = r + jn
            
            var n = _curve.N;  // Curve order.
            var i = BigInteger.ValueOf((long)recId / 2);
            var x = r.Add(i.Multiply(n));
            
            //   1.2. Convert the integer x to an octet string X of length mLen using the conversion
            //        routine specified in Section 2.3.7, where mLen = ⌈(log2 p)/8⌉ or mLen = ⌈m/8⌉.
            //   1.3. Convert the octet string (16 set binary digits)||X to an elliptic curve point R
            //        using the conversion routine specified in Section 2.3.4. If this conversion
            //        routine outPuts "invalid", then do another iteration of Step 1.
            //
            // More concisely, what these points mean is to use X as a compressed public key.
            
            var prime = SecP256K1CurveQ;
            if (x.CompareTo(prime) >= 0)
            {
                // Cannot have point co-ordinates larger than this as everything takes place modulo Q.
                return null;
            }
            
            // Compressed keys require you to know an extra bit of data about the y-coord as there are
            // two possibilities. So it's encoded in the recId.
            
            var ecPoint = DecompressKey(x, (recId & 1) == 1);
            
            //   1.4. If nR != point at infinity, then do another iteration of Step 1 (callers
            //        responsibility).
            
            if (!ecPoint.Multiply(n).IsInfinity)
            {
                return null;
            }
            
            //   1.5. Compute e from M using Steps 2 and 3 of ECDSASignature signature verification.
            
            var e = new BigInteger(1, message);
            
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
            
            var eInv = BigInteger.Zero.Subtract(e).Mod(n);
            var rInv = r.ModInverse(n);
            var srInv = rInv.Multiply(s).Mod(n);
            var eInvrInv = rInv.Multiply(eInv).Mod(n);
            var q = ECAlgorithms.SumOfTwoMultiplies(_curve.G, eInvrInv, ecPoint, srInv);

            var qBytes = q.GetEncoded(false);
            
            // We remove the prefix
            
            return new BigInteger(1, Arrays.CopyOfRange(qBytes, 1, qBytes.Length));
        }

        private ECPoint DecompressKey(BigInteger xBn, bool yBit)
        {
            var byteLength = 1 + X9IntegerConverter.GetByteLength(_curve.Curve);
            var compEnc = X9IntegerConverter.IntegerToBytes(xBn, byteLength);
            compEnc[0] = (byte)(yBit ? 0x03 : 0x02);
            return _curve.Curve.DecodePoint(compEnc);
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private static void CheckArgument(bool expression, string message)
        {
            if (!expression)
            {
                throw new ArgumentException(message);
            }
        }
    }
}