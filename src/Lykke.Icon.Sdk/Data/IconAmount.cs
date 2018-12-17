using System;
using System.Numerics;

namespace Lykke.Icon.Sdk.Data
{
    public class IconAmount
    {
        private BigDecimal _value;
        private readonly int _digit;

        public IconAmount(BigDecimal value, int digit)
        {
            this._value = value;
            this._digit = digit;
        }

        public int GetDigit()
        {
            return _digit;
        }

        public override String ToString()
        {
            return _value.ToString();
        }

        public BigInteger ToInteger()
        {
            return _value.ToBigInteger();
        }

        public BigDecimal ToDecimal()
        {
            return _value;
        }

        public BigInteger ToLoop()
        {
            var tenDigit = GetTenDigit(_digit);
            return (_value * tenDigit).ToBigInteger();
        }

        public IconAmount ConvertUnit(Unit unit)
        {
            BigInteger loop = ToLoop();
            return IconAmount.Of(new BigDecimal(loop) / GetTenDigit(unit.GetValue()), unit.GetValue());
        }

        public IconAmount ConvertUnit(int digit)
        {
            BigInteger loop = ToLoop();
            return IconAmount.Of(new BigDecimal(loop) / (GetTenDigit(digit)), digit);
        }

        private BigDecimal GetTenDigit(int digit)
        {
            var result = new BigDecimal(1);
            var value = new BigDecimal(10);

            for (int i = 0; i < digit; i++)
            {
                result = result * value;
            }

            return result;
        }

        public class Unit
        {
            public const int LOOP = 0;
            public const int ICX = 18;

            int _digit;

            Unit(int digit)
            {
                this._digit = digit;
            }

            public int GetValue()
            {
                return _digit;
            }
        }

        public static IconAmount Of(BigDecimal loop, int digit)
        {
            return new IconAmount(loop, digit);
        }

        public static IconAmount Of(decimal loop, Unit unit)
        {
            return Of(loop, unit.GetValue());
        }

        public static IconAmount Of(String loop, int digit)
        {
            return Of(BigDecimal.Parse(loop), digit);
        }

        public static IconAmount Of(String loop, Unit unit)
        {
            return Of(BigDecimal.Parse(loop), unit.GetValue());
        }

        public static IconAmount Of(BigInteger loop, int digit)
        {
            return Of(new BigDecimal(loop, 0), digit);
        }

        public static IconAmount Of(BigInteger loop, Unit unit)
        {
            return Of(new BigDecimal(loop), unit.GetValue());
        }
    }
}
