using System.Globalization;
using System.Numerics;
using Lykke.Icon.Sdk.Crypto;
using Lykke.Icon.Sdk.Data;
using Lykke.Icon.Sdk.Transport.JsonRpc;
using Org.BouncyCastle.Utilities.Encoders;
using Xunit;

namespace Lykke.Icon.Sdk.Tests.Crypto
{
    public class SignVerificationTest
    {
        private IWallet wallet;

        public SignVerificationTest()
        {
            wallet = KeyWallet.Load(new Bytes(SampleKeys.PrivateKeyString));
        }

        [Fact]
        public void CheckVerificationPositive()
        {
            var from = wallet.GetAddress();
            var to = new Address("hx5bfdb090f43a808005ffc27c25b213145e80b7cd");

            var transaction = TransactionBuilder.CreateBuilder()
                    .Nid(NetworkId.Main)
                    .From(from)
                    .To(to)
                    .Value(BigInteger.Parse("0de0b6b3a7640000", NumberStyles.AllowHexSpecifier))
                    .StepLimit(BigInteger.Parse("12345", NumberStyles.AllowHexSpecifier))
                    .Timestamp(BigInteger.Parse("563a6cf330136", NumberStyles.AllowHexSpecifier))
                    .Nonce(BigInteger.Parse("1"))
                    .Build();

            var signedTransaction = new SignedTransaction(transaction, wallet);
            var transactionProps = signedTransaction.GetTransactionProperties();
            var allProps = signedTransaction.GetProperties();
            var hash = SignedTransaction.GetTransactionHash(transactionProps);
            var signature = Base64.Decode(allProps.GetItem("signature").ToString());
            var result = EcdsaSignature.VerifySignature(from, signature, hash);
            Assert.True(result);
        }

        [Fact]
        public void CheckVerificationNegative()
        {
            var randomKey = KeyWallet.Create();
            var from = wallet.GetAddress();
            var to = new Address("hx5bfdb090f43a808005ffc27c25b213145e80b7cd");

            var transaction = TransactionBuilder.CreateBuilder()
                .Nid(NetworkId.Main)
                .From(from)
                .To(to)
                .Value(BigInteger.Parse("0de0b6b3a7640000", NumberStyles.AllowHexSpecifier))
                .StepLimit(BigInteger.Parse("12345", NumberStyles.AllowHexSpecifier))
                .Timestamp(BigInteger.Parse("563a6cf330136", NumberStyles.AllowHexSpecifier))
                .Nonce(BigInteger.Parse("1"))
                .Build();

            var signedTransaction = new SignedTransaction(transaction, wallet);
            var transactionProps = signedTransaction.GetTransactionProperties();
            var allProps = signedTransaction.GetProperties();
            var hash = SignedTransaction.GetTransactionHash(transactionProps);
            var strSign = allProps.GetItem("signature").ToString();
            var serialized = SignedTransaction.Serialize(allProps);
            var signature = Base64.Decode(strSign);
            var result = EcdsaSignature.VerifySignature(randomKey.GetAddress(), signature, hash);
            Assert.False(result);
        }

        [Fact]
        public void CheckVerificationMalformedSignature()
        {
            var randomKey = KeyWallet.Create();
            var from = wallet.GetAddress();
            var to = new Address("hx5bfdb090f43a808005ffc27c25b213145e80b7cd");

            var transaction = TransactionBuilder.CreateBuilder()
                .Nid(NetworkId.Main)
                .From(from)
                .To(to)
                .Value(BigInteger.Parse("0de0b6b3a7640000", NumberStyles.AllowHexSpecifier))
                .StepLimit(BigInteger.Parse("12345", NumberStyles.AllowHexSpecifier))
                .Timestamp(BigInteger.Parse("563a6cf330136", NumberStyles.AllowHexSpecifier))
                .Nonce(BigInteger.Parse("1"))
                .Build();

            var signedTransaction = new SignedTransaction(transaction, wallet);
            var transactionProps = signedTransaction.GetTransactionProperties();
            var allProps = signedTransaction.GetProperties();
            var hash = SignedTransaction.GetTransactionHash(transactionProps);
            var signature = Base64.Decode(allProps.GetItem("signature").ToString());
            signature[64] = 212;
            var result = EcdsaSignature.VerifySignature(randomKey.GetAddress(), signature, hash);
            Assert.False(result);
        }
    }
}
