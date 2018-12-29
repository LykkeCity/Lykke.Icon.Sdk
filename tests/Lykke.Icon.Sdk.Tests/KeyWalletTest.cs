using System;
using Lykke.Icon.Sdk.Crypto;
using Lykke.Icon.Sdk.Data;
using Xunit;

namespace Lykke.Icon.Sdk.Tests
{
    public class KeyWalletTest
    {
        [Fact]
        public void TestLoadWithPrivateKey()
        {
            var pkBytes = new Bytes(SampleKeys.PrivateKeyString);
            var wallet = KeyWallet.Load(pkBytes);
            var loadedAddress = wallet.GetAddress();
            Assert.Equal(new Address(SampleKeys.Address), loadedAddress);
        }

        [Fact]
        public void TestCreate()
        {
            var wallet = KeyWallet.Create();
            Assert.True(IconKeys.IsValidAddress(wallet.GetAddress().ToString()));
            IWallet loadWallet = KeyWallet.Load(wallet.GetPrivateKey());
            Assert.Equal(wallet.GetAddress(), loadWallet.GetAddress());
        }

        [Fact]
        private void TestSignMessage()
        {
            const string message = "0xefc935bb4a944ccf02b4ff4a601f5bb47d60b55e21aa9683aaf17bf1d79129ae";
            const string expected = "0x7e224bd64f2fa18a340acda4f7e567f87d9c8e65e523759d00034453b92be2d55ab206c41bc60f831055ae2f49ab40431a209a87f09a965492a84ab1f0b885c001";

            var privateKey = new Bytes(SampleKeys.PrivateKeyString);
            var wallet = KeyWallet.Load(privateKey);
            var byteArray = new Bytes(message).ToByteArray();
            var sign = wallet.Sign(byteArray);
            var hexString = new Bytes(sign).ToHexString(true);
            Assert.Equal(expected, hexString);
        }

        [Fact]
        public void TestSignMessageNullException()
        {
            var wallet = KeyWallet.Load(new Bytes(SampleKeys.PrivateKeyString));
            byte[] message = null;
            Assert.Throws<ArgumentException>(() =>
            {
                // ReSharper disable once ExpressionIsAlwaysNull
                wallet.Sign(message);
            });
        }
    }
}
