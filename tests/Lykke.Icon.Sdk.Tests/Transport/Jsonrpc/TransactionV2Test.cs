using Lykke.Icon.Sdk.Data;
using Lykke.Icon.Sdk.Transport.JsonRpc;
using System.Globalization;
using System.Numerics;
using Xunit;

namespace Lykke.Icon.Sdk.Tests.Transport.Jsonrpc
{
    public class TransactionV2Test
    {
        [Fact]
        public void TestVersion3()
        {
            RpcObject @object = new RpcObject.Builder()
                .Put("timestamp", new RpcValue("1535964734110836"))
                .Put("nonce", new RpcValue("8367273"))
                .Put("value", new RpcValue("4563918244f40000"))
                .Put("version", new RpcValue("0x3"))
                .Build();

            ConfirmedTransaction tx = Converters.CONFIRMED_TRANSACTION.ConvertTo(@object);
            Assert.Throws<RpcValueException>(() =>
            {
                tx.GetTimestamp();
            });

            Assert.Throws<RpcValueException>(() =>
            {
                tx.GetNonce();
            });

            Assert.Throws<RpcValueException>(() =>
            {
                tx.GetValue();
            });

            Assert.True(tx.GetFee() == 0);
        }

        [Fact]
        public void TestChangeSpec()
        {
            RpcObject @object = new RpcObject.Builder()
                .Put("timestamp", new RpcValue(BigInteger.Parse("1535964734110836")))
                .Put("nonce", new RpcValue("8367273"))
                .Put("value", new RpcValue("0x4563918244f40000"))
                .Put("fee", new RpcValue("0x2386f26fc10000"))
                .Put("tx_hash", new RpcValue("30c19ce2b5139ead7fb51c567cf8a01a3ca8d7f881c04f1312b3550330c690bb"))
                .Build();

            ConfirmedTransaction tx = Converters.CONFIRMED_TRANSACTION.ConvertTo(@object);
            Assert.Equal(BigInteger.Parse("2", NumberStyles.HexNumber), tx.GetVersion());
            Assert.Equal(BigInteger.Parse("4563918244f40000", NumberStyles.HexNumber), tx.GetValue());
            Assert.Equal(BigInteger.Parse("1535964734110836"), tx.GetTimestamp());
            Assert.Equal(BigInteger.Parse("8367273"), tx.GetNonce());
            Assert.Equal(BigInteger.Parse("2386f26fc10000", NumberStyles.HexNumber), tx.GetFee());
            var expected = new Bytes("30c19ce2b5139ead7fb51c567cf8a01a3ca8d7f881c04f1312b3550330c690bb");
            var txHash = tx.GetTxHash();
            Assert.Equal(expected, txHash);
        }

        [Fact]
        public void TestNoPrefixValue()
        {
            RpcObject @object = new RpcObject.Builder()
                .Put("value", new RpcValue("4563918244f40000"))
                .Put("fee", new RpcValue("2386f26fc10000"))
                .Build();

            ConfirmedTransaction tx = Converters.CONFIRMED_TRANSACTION.ConvertTo(@object);
            Assert.Equal(BigInteger.Parse("2", NumberStyles.HexNumber), tx.GetVersion());
            Assert.Equal(BigInteger.Parse("4563918244f40000", NumberStyles.HexNumber), tx.GetValue());
            Assert.Equal(BigInteger.Parse("2386f26fc10000", NumberStyles.HexNumber), tx.GetFee());
        }

        [Fact]
        public void TestInvalidValue()
        {
            RpcObject @object = new RpcObject.Builder()
                .Put("to", new RpcValue("123124124124"))
                .Put("timestamp", new RpcValue(""))
                .Build();

            ConfirmedTransaction tx = Converters.CONFIRMED_TRANSACTION.ConvertTo(@object);

            Assert.Equal("123124124124", tx.GetTo().ToString());
            //Assert.Null(tx.GetTimestamp());

            @object = new RpcObject.Builder()
                .Put("to", new RpcValue("bf85fac2d0b507a2db9ce9526e6d91476f16a2d269f51636f9c4b2d512017faf"))
                .Put("timestamp", null)
                .Build();
            tx = Converters.CONFIRMED_TRANSACTION.ConvertTo(@object);
            Assert.Equal("bf85fac2d0b507a2db9ce9526e6d91476f16a2d269f51636f9c4b2d512017faf", tx.GetTo().ToString());
            //Assert.Null(tx.GetTimestamp());

            @object = new RpcObject.Builder()
                .Put("to", new RpcValue(""))
                .Put("value", new RpcValue("45400a8fd5330000"))
                .Build();
            tx = Converters.CONFIRMED_TRANSACTION.ConvertTo(@object);
            Assert.Null(tx.GetTo());
            Assert.Equal(BigInteger.Parse("45400a8fd5330000", NumberStyles.HexNumber), tx.GetValue());

            @object = new RpcObject.Builder()
                .Put("to", new RpcValue("hxa23651905d221dd36b"))
                .Put("timestamp", new RpcValue(1535964734110836L.ToString()))
                .Build();
            tx = Converters.CONFIRMED_TRANSACTION.ConvertTo(@object);
            Assert.Equal("hxa23651905d221dd36b", tx.GetTo().ToString());
            Assert.Equal("1535964734110836", tx.GetTimestamp().ToString());
        }
    }
}
