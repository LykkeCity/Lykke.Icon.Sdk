using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Icon.Sdk.Crypto;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk.Data
{
    public class IconAmount
    {

        private BigDecimal value;
        private int digit;

        public IconAmount(BigDecimal value, int digit)
        {
            this.value = value;
            this.digit = digit;
        }

        public int GetDigit()
        {
            return digit;
        }

        public String ToString()
        {
            return value.toString();
        }

        public BigInteger ToInteger()
        {
            return value.ToBigInteger();
        }

        public BigDecimal ToDecimal()
        {
            return value;
        }

        public BigInteger ToLoop()
        {
            return value.multiply(GetTenDigit(digit)).ToBigInteger();
        }

        public IconAmount ConvertUnit(Unit unit)
        {
            BigInteger loop = ToLoop();
            return IconAmount.of(new BigDecimal(loop).divide(getTenDigit(unit.getValue())), unit);
        }

        public IconAmount ConvertUnit(int digit)
        {
            BigInteger loop = toLoop();
            return IconAmount.of(new BigDecimal(loop).divide(getTenDigit(digit)), digit);
        }

        private BigDecimal GetTenDigit(int digit)
        {
            return BigDecimal.TEN.pow(digit);
        }

        public enum Unit
        {
            LOOP(0),
        ICX(18);

        int digit;

        Unit(int digit)
        {
            this.digit = digit;
        }

        public int GetValue()
        {
            return digit;
        }
    }

    public static IconAmount Of(BigDecimal loop, int digit)
    {
        return new IconAmount(loop, digit);
    }

    public static IconAmount Of(BigDecimal loop, Unit unit)
    {
        return Of(loop, unit.getValue());
    }

    public static IconAmount Of(String loop, int digit)
    {
        return Of(new BigDecimal(loop), digit);
    }

    public static IconAmount Of(String loop, Unit unit)
    {
        return Of(new BigDecimal(loop), unit.getValue());
    }

    public static IconAmount Of(BigInteger loop, int digit)
    {
        return Of(new BigDecimal(loop), digit);
    }

    public static IconAmount Of(BigInteger loop, Unit unit)
    {
        return Of(new BigDecimal(loop), unit.getValue());
    }
}
