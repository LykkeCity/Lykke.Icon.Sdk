using System;
using Lykke.Icon.Sdk.Crypto;
using Lykke.Icon.Sdk.Data;
using Xunit;

namespace Lykke.Icon.Sdk.Tests.Data
{
    public class AddressTest
    {
        private String eoa = "hx4873b94352c8c1f3b2f09aaeccea31ce9e90bd31";
        private String contract = "cx1ca4697e8229e29adce3cded4412a137be6d7edb";

        [Fact]
        public void TestEoaAddress()
        {
            Address address = new Address(eoa);
            var prefix = address.GetPrefix().ToString();
            Assert.Equal(eoa, address.ToString());
            Assert.Equal(Address.AddressPrefix.EOA, prefix);
            Assert.True(IconKeys.IsValidAddress(address));
            Assert.False(IconKeys.IsContractAddress(address));
        }

        [Fact]
        void TestContractCreate()
        {
            Address address = new Address(contract);
            Assert.Equal(contract, address.ToString());
            Assert.Equal(Address.AddressPrefix.CONTRACT, address.GetPrefix().ToString());
            Assert.True(IconKeys.IsValidAddress(address));
            Assert.True(IconKeys.IsContractAddress(address));
        }

        [Fact]
        void TestInvalidCreate()
        {
            String noPrefix = "4873b94352c8c1f3b2f09aaeccea31ce9e90bd31";
            Assert.Throws<ArgumentException>(() =>
            {
                new Address(noPrefix);
            });

            String missCharacter = "4873b94352c8c1f3b2f09aaeccea31ce9e90bd3";
            Assert.Throws<ArgumentException>(() =>
            {
                new Address(missCharacter);
            });

            String notHex = "4873b94352c8c1f3b2f09aaeccea31ce9e90bd3g";
            Assert.Throws<ArgumentException>(() =>
            {
                new Address(notHex);
            });

            String words = "helloworldhelloworldhelloworldhelloworld";
            Assert.Throws<ArgumentException>(() =>
            {
                new Address(words);
            });

            String upperAddress = "hx" + noPrefix.ToUpperInvariant();
            Assert.Throws<ArgumentException>(() =>
            {
                new Address(upperAddress);
            });
        }


    }
}