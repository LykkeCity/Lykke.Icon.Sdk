using Lykke.Icon.Sdk.Crypto;
using Lykke.Icon.Sdk.Data;
using System;
using Xunit;

namespace Lykke.Icon.Sdk.Tests
{
    public class KeyWalletTest
    {
        [Fact]
        public void TestLoadWithPrivateKey()
        {
            var pkBytes = new Bytes(SampleKeys.PRIVATE_KEY_STRING);
            KeyWallet wallet = KeyWallet.Load(pkBytes);
            var loadedAddress = wallet.GetAddress();
            Assert.Equal(new Address(SampleKeys.ADDRESS), loadedAddress);
        }

        [Fact]
        public void TestCreate()
        {
            KeyWallet wallet = KeyWallet.Create();
            Assert.True(IconKeys.IsValidAddress(wallet.GetAddress().ToString()));
            Wallet loadWallet = KeyWallet.Load(wallet.GetPrivateKey());
            Assert.Equal(wallet.GetAddress(), loadWallet.GetAddress());
        }

        [Fact]
        void TestSignMessage()
        {
            String message = "0xefc935bb4a944ccf02b4ff4a601f5bb47d60b55e21aa9683aaf17bf1d79129ae";
            String expected = "0x7e224bd64f2fa18a340acda4f7e567f87d9c8e65e523759d00034453b92be2d55ab206c41bc60f831055ae2f49ab40431a209a87f09a965492a84ab1f0b885c001";

            var privateKey = new Bytes(SampleKeys.PRIVATE_KEY_STRING);
            KeyWallet wallet = KeyWallet.Load(privateKey);
            var byteArray = new Bytes(message).ToByteArray();
            byte[] sign = wallet.Sign(byteArray);
            var hexString = new Bytes(sign).ToHexString(true);
            Assert.Equal(expected, hexString);
        }

        [Fact]
        void TestSignMessageNullException()
        {
            KeyWallet wallet = KeyWallet.Load(new Bytes(SampleKeys.PRIVATE_KEY_STRING));
            byte[] message = null;
            Assert.Throws<ArgumentException>(() =>
            {
                wallet.Sign(message);
            });
        }
    }
}
