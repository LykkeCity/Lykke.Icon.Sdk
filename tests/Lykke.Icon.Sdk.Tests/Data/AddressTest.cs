using System;
using System.Diagnostics.CodeAnalysis;
using Lykke.Icon.Sdk.Crypto;
using Lykke.Icon.Sdk.Data;
using Xunit;

namespace Lykke.Icon.Sdk.Tests.Data
{
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class AddressTest
    {
        private const string Eoa = "hx4873b94352c8c1f3b2f09aaeccea31ce9e90bd31";
        private const string Contract = "cx1ca4697e8229e29adce3cded4412a137be6d7edb";

        [Fact]
        public void TestEoaAddress()
        {
            var address = new Address(Eoa);
            var prefix = address.GetPrefix().ToString();
            Assert.Equal(Eoa, address.ToString());
            Assert.Equal(AddressPrefix.Eoa, prefix);
            Assert.True(IconKeys.IsValidAddress(address));
            Assert.False(IconKeys.IsContractAddress(address));
        }

        [Fact]
        private void TestContractCreate()
        {
            var address = new Address(Contract);
            Assert.Equal(Contract, address.ToString());
            Assert.Equal(AddressPrefix.Contract, address.GetPrefix().ToString());
            Assert.True(IconKeys.IsValidAddress(address));
            Assert.True(IconKeys.IsContractAddress(address));
        }

        [Fact]
        private void TestInvalidCreate()
        {
            const string noPrefix = "4873b94352c8c1f3b2f09aaeccea31ce9e90bd31";
            
            Assert.Throws<ArgumentException>(() =>
            {
                new Address(noPrefix);
            });

            var missCharacter = "4873b94352c8c1f3b2f09aaeccea31ce9e90bd3";
            Assert.Throws<ArgumentException>(() =>
            {
                new Address(missCharacter);
            });

            var notHex = "4873b94352c8c1f3b2f09aaeccea31ce9e90bd3g";
            Assert.Throws<ArgumentException>(() =>
            {
                new Address(notHex);
            });

            var words = "helloworldhelloworldhelloworldhelloworld";
            Assert.Throws<ArgumentException>(() =>
            {
                new Address(words);
            });

            var upperAddress = "hx" + noPrefix.ToUpperInvariant();
            Assert.Throws<ArgumentException>(() =>
            {
                new Address(upperAddress);
            });
        }


    }
}