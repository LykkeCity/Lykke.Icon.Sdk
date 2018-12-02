using System.Numerics;
using Lykke.Icon.Sdk.Data;
using Xunit;

namespace Lykke.Icon.Sdk.Tests.Data
{
    public class IconAmountTest
    {
        [Fact]
        public void TestCreate()
        {
            BigInteger loop = BigInteger.Parse("1000000000000000000");

            IconAmount amount = IconAmount.Of("1", IconAmount.Unit.ICX);
            Assert.Equal(BigInteger.Parse("1"), amount.ToInteger());
            Assert.Equal(IconAmount.Unit.ICX, amount.GetDigit());

            amount = IconAmount.Of("1000000000000000000", IconAmount.Unit.LOOP);
            Assert.Equal("1000000000000000000", amount.ToString());
            Assert.Equal(IconAmount.Unit.LOOP, amount.GetDigit());

            amount = IconAmount.Of(BigInteger.Parse("1000000000000000000"), 16);
            Assert.Equal(BigInteger.Parse("1000000000000000000"), amount.ToInteger());
            Assert.Equal(16, amount.GetDigit());

            amount = IconAmount.Of(BigDecimal.Parse("0.1"), IconAmount.Unit.ICX);
            Assert.Equal(BigDecimal.Parse("0.1"), amount.ToDecimal());
            Assert.Equal(IconAmount.Unit.ICX, amount.GetDigit());

            amount = IconAmount.Of(BigDecimal.Parse("0.1"), 16);
            Assert.Equal(BigDecimal.Parse("0.1"), amount.ToDecimal());
            Assert.Equal(16, amount.GetDigit());
        }

        [Fact]
        public void TestToLoop()
        {
            BigInteger loop = BigInteger.Parse("1000000000000000000");

            IconAmount amount = IconAmount.Of("1", IconAmount.Unit.ICX);
            Assert.Equal(loop, amount.ToLoop());

            amount = IconAmount.Of("1000000000000000000", IconAmount.Unit.LOOP);
            Assert.Equal(loop, amount.ToLoop());

            amount = IconAmount.Of(BigInteger.Parse("1"), IconAmount.Unit.ICX);
            Assert.Equal(loop, amount.ToLoop());

            amount = IconAmount.Of(BigInteger.Parse("1000"), IconAmount.Unit.ICX);
            Assert.Equal(BigInteger.Parse("1000000000000000000000"), amount.ToLoop());

            amount = IconAmount.Of("0.1", IconAmount.Unit.ICX);
            var loop2 = amount.ToLoop();
            Assert.Equal(BigInteger.Parse("100000000000000000"), loop2);

            amount = IconAmount.Of(BigDecimal.Parse("0.1"), IconAmount.Unit.ICX);
            Assert.Equal(BigInteger.Parse("100000000000000000"), amount.ToLoop());
        }

        [Fact]
        public void TestConvertUnit()
        {

            BigDecimal loop = BigDecimal.Parse("1000000000000000000");

            IconAmount amount = IconAmount.Of("1", IconAmount.Unit.ICX);
            Assert.Equal(BigInteger.Parse("10"), amount.ConvertUnit(17).ToInteger());

            amount = IconAmount.Of("1", IconAmount.Unit.ICX);
            Assert.Equal(BigInteger.Parse("100"), amount.ConvertUnit(16).ToInteger());

            amount = IconAmount.Of(BigDecimal.Parse("1"), IconAmount.Unit.ICX);
            Assert.Equal(BigDecimal.Parse("0.1"), amount.ConvertUnit(19).ToDecimal());

            amount = IconAmount.Of(BigDecimal.Parse("1"), IconAmount.Unit.ICX);
            Assert.Equal(BigInteger.Parse("1000000000000000000"), amount.ConvertUnit(IconAmount.Unit.LOOP).ToInteger());
        }
    }
}
