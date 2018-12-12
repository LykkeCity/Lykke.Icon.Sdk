using System;
using System.Globalization;
using System.Linq;
using Lykke.Icon.Sdk.Transport.JsonRpc;
using System.Numerics;
using Xunit;

namespace Lykke.Icon.Sdk.Tests.Transport.Jsonrpc
{
    public class RpcValueTest
    {
        private RpcValue plainStringValue;
        private RpcValue bytesValue;
        private RpcValue oddIntegerValue;
        private RpcValue evenIntegerValue;
        private RpcValue booleanValue;

        public RpcValueTest()
        {
            plainStringValue = new RpcValue("string value");
            bytesValue = new RpcValue(new byte[] { 1, 2, 3, 4, 5 });
            oddIntegerValue = new RpcValue(BigInteger.Parse("1234"));
            evenIntegerValue = new RpcValue(BigInteger.Parse("61731"));
            booleanValue = new RpcValue(true);
        }

        [Fact]
        public void TestAsString()
        {
            Assert.Equal("string value", plainStringValue.ToString());
            Assert.Equal("0x0102030405", bytesValue.ToString());
            Assert.Equal("0x4d2", oddIntegerValue.ToString());
            Assert.Equal("0xf123", evenIntegerValue.ToString());
            Assert.Equal("0x1", booleanValue.ToString());
        }

        [Fact]
        public void TestAsBytes()
        {
            Assert.Throws<RpcValueException>(() => plainStringValue.ToByteArray());
            Assert.True(Enumerable.SequenceEqual(new byte[] { 1, 2, 3, 4, 5 }, bytesValue.ToByteArray()));
            Assert.Throws<RpcValueException>(() => oddIntegerValue.ToByteArray());
            Assert.True(Enumerable.SequenceEqual(new byte[] { 241, 35 }, evenIntegerValue.ToByteArray()));
            Assert.Throws<RpcValueException>(() => booleanValue.ToByteArray());
        }

        [Fact]
        public void TestAsInteger()
        {
            Assert.Throws<RpcValueException>(() => plainStringValue.ToInteger());
            Assert.Equal(BigInteger.Parse("0102030405", NumberStyles.HexNumber), bytesValue.ToInteger());
            Assert.Equal(BigInteger.Parse("1234"), oddIntegerValue.ToInteger());
            var intValue = evenIntegerValue.ToInteger();
            Assert.Equal(BigInteger.Parse("61731"), intValue);
            Assert.Equal(BigInteger.Parse("1"), booleanValue.ToInteger());

            RpcValue minusHex = new RpcValue("-0x4d2");
            Assert.Equal(BigInteger.Parse("-1234"), minusHex.ToInteger());

            RpcValue plusHex = new RpcValue("+0x4d2");
            Assert.Throws<RpcValueException>(() => plusHex.ToInteger());
        }

        [Fact]
        public void TestAsBoolean()
        {
            Assert.Throws<RpcValueException>(() => plainStringValue.ToBoolean());
            Assert.Throws<RpcValueException>(() => bytesValue.ToBoolean());
            Assert.Throws<RpcValueException>(() => oddIntegerValue.ToBoolean());
            Assert.Throws<RpcValueException>(() => evenIntegerValue.ToBoolean());
            Assert.True(booleanValue.ToBoolean());
        }
    }
}