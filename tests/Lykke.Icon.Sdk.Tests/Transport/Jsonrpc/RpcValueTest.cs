using System.Globalization;
using System.Linq;
using System.Numerics;
using Lykke.Icon.Sdk.Transport.JsonRpc;
using Xunit;

namespace Lykke.Icon.Sdk.Tests.Transport.Jsonrpc
{
    public class RpcValueTest
    {
        private readonly RpcValue _plainStringValue;
        private readonly RpcValue _bytesValue;
        private readonly RpcValue _oddIntegerValue;
        private readonly RpcValue _evenIntegerValue;
        private readonly RpcValue _booleanValue;

        public RpcValueTest()
        {
            _plainStringValue = new RpcValue("string value");
            _bytesValue = new RpcValue(new byte[] { 1, 2, 3, 4, 5 });
            _oddIntegerValue = new RpcValue(BigInteger.Parse("1234"));
            _evenIntegerValue = new RpcValue(BigInteger.Parse("61731"));
            _booleanValue = new RpcValue(true);
        }

        [Fact]
        public void TestAsString()
        {
            Assert.Equal("string value", _plainStringValue.ToString());
            Assert.Equal("0x0102030405", _bytesValue.ToString());
            Assert.Equal("0x4d2", _oddIntegerValue.ToString());
            Assert.Equal("0xf123", _evenIntegerValue.ToString());
            Assert.Equal("0x1", _booleanValue.ToString());
        }

        [Fact]
        public void TestAsBytes()
        {
            Assert.Throws<RpcValueException>(() => _plainStringValue.ToByteArray());
            Assert.True(new byte[] { 1, 2, 3, 4, 5 }.SequenceEqual(_bytesValue.ToByteArray()));
            Assert.Throws<RpcValueException>(() => _oddIntegerValue.ToByteArray());
            Assert.True(new byte[] { 241, 35 }.SequenceEqual(_evenIntegerValue.ToByteArray()));
            Assert.Throws<RpcValueException>(() => _booleanValue.ToByteArray());
        }

        [Fact]
        public void TestAsInteger()
        {
            Assert.Throws<RpcValueException>(() => _plainStringValue.ToInteger());
            Assert.Equal(BigInteger.Parse("0102030405", NumberStyles.HexNumber), _bytesValue.ToInteger());
            Assert.Equal(BigInteger.Parse("1234"), _oddIntegerValue.ToInteger());
            var intValue = _evenIntegerValue.ToInteger();
            Assert.Equal(BigInteger.Parse("61731"), intValue);
            Assert.Equal(BigInteger.Parse("1"), _booleanValue.ToInteger());

            var minusHex = new RpcValue("-0x4d2");
            Assert.Equal(BigInteger.Parse("-1234"), minusHex.ToInteger());

            var plusHex = new RpcValue("+0x4d2");
            Assert.Throws<RpcValueException>(() => plusHex.ToInteger());
        }

        [Fact]
        public void TestAsBoolean()
        {
            Assert.Throws<RpcValueException>(() => _plainStringValue.ToBoolean());
            Assert.Throws<RpcValueException>(() => _bytesValue.ToBoolean());
            Assert.Throws<RpcValueException>(() => _oddIntegerValue.ToBoolean());
            Assert.Throws<RpcValueException>(() => _evenIntegerValue.ToBoolean());
            Assert.True(_booleanValue.ToBoolean());
        }
    }
}