using System.Globalization;
using System.Text;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Numerics
{
    [PublicAPI]
    public struct BigDecimal
    {
        private static readonly BigInteger MaxPrecision = 20;
        
        private BigInteger _value;
        private ushort _scale;

        public BigDecimal(float value)
        {
            this = Parse(value.ToString("R"));
        }

        public BigDecimal(double value)
        {
            this = Parse(value.ToString("R"));
        }

        public BigDecimal(decimal value)
        {
            this = Parse(value.ToString(CultureInfo.InvariantCulture));
        }

        public BigDecimal(long value)
        {
            this = new BigDecimal((BigInteger)value);
        }

        public BigDecimal(ulong value)
        {
            this = new BigDecimal((BigInteger)value);
        }

        public BigDecimal(BigInteger value)
        {
            this = new BigDecimal(value, 0);
        }

        public BigDecimal(BigInteger value, ushort scale)
        {
            _value = value;
            _scale = scale;
        }

        [Pure]
        public BigInteger ToBigInteger()
        {
            var scaleDivisor = BigInteger.Pow(new BigInteger(10), _scale);
            var scaledValue = BigInteger.Divide(_value, scaleDivisor);
            
            return scaledValue;
        }

        public override string ToString()
        {
            if (_scale == 0)
            {
                return _value.ToString();
            }
            
            var str = _value.ToString();
            
            if (str.Length > _scale)
            {
                return str.Insert(str.Length - _scale, ".");
            }
            
            return "0." + new string('0', _scale - str.Length) + str;
        }

        private BigDecimal Upscale(ushort newScale)
        {
            if (newScale < _scale)
            {
                throw new InvalidOperationException("Cannot upscale a BigDecimal to a smaller scale!");
            }
            
            return new BigDecimal(_value * BigInteger.Pow(10, newScale - _scale), newScale);
        }

        public static BigDecimal Parse(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str), "BigDecimal.Parse: Cannot parse null");
            }
            
            ushort scale = 0;
            var stringBuilder = new StringBuilder();
            var exponentBuilder = (StringBuilder)null;
            var state = ParseState.Start;
            
            var action1 = (Action<char>)(c => throw new FormatException("BigDecimal.Parse: invalid character '" + c + "' in: " + str));
            var action2 = (Action)(() =>
            {
                exponentBuilder = new StringBuilder();
                state = ParseState.E;
            });
            
            foreach (var c in str)
            {
                switch (state)
                {
                    case ParseState.Start:
                        if (!char.IsDigit(c))
                        {
                            switch (c)
                            {
                                case '+':
                                case '-':
                                    break;
                                case '.':
                                    state = ParseState.Decimal;
                                    continue;
                                default:
                                    action1(c);
                                    continue;
                            }
                        }
                        state = ParseState.Integer;
                        stringBuilder.Append(c);
                        break;
                    case ParseState.Integer:
                        if (char.IsDigit(c))
                        {
                            stringBuilder.Append(c);
                            break;
                        }
                        switch (c)
                        {
                            case '.':
                                state = ParseState.Decimal;
                                continue;
                            case 'E':
                            case 'e':
                                action2();
                                continue;
                            default:
                                action1(c);
                                continue;
                        }
                    case ParseState.Decimal:
                        if (char.IsDigit(c))
                        {
                            checked { ++scale; }
                            stringBuilder.Append(c);
                            break;
                        }
                        if (c == 'e' || c == 'E')
                        {
                            action2();
                            break;
                        }
                        action1(c);
                        break;
                    case ParseState.E:
                        if (char.IsDigit(c) || c == '-' || c == '+')
                        {
                            state = ParseState.Exponent;
                            exponentBuilder.Append(c);
                            break;
                        }
                        action1(c);
                        break;
                    case ParseState.Exponent:
                        if (char.IsDigit(c))
                        {
                            exponentBuilder.Append(c);
                            break;
                        }
                        action1(c);
                        break;
                }
            }

            if (stringBuilder.Length == 0 || stringBuilder.Length == 1 && !char.IsDigit(stringBuilder[0]))
            {
                throw new FormatException("BigDecimal.Parse: string didn't contain a value: \"" + str + "\"");
            }

            if (exponentBuilder != null && (exponentBuilder.Length == 0 || stringBuilder.Length == 1 && !char.IsDigit(stringBuilder[0])))
            {
                throw new FormatException("BigDecimal.Parse: string contained an 'E' but no exponent value: \"" + str + "\"");
            }
            
            var bigInteger = BigInteger.Parse(stringBuilder.ToString());
            
            if (exponentBuilder != null)
            {
                var num1 = int.Parse(exponentBuilder.ToString());
                if (num1 > 0)
                {
                    if (num1 <= scale)
                    {
                        scale -= (ushort)num1;
                    }
                    else
                    {
                        var exponent = num1 - scale;
                        scale = 0;
                        bigInteger *= BigInteger.Pow(10, exponent);
                    }
                }
                else if (num1 < 0)
                {
                    var num2 = -num1 + scale;
                    if (num2 <= ushort.MaxValue)
                    {
                        scale = (ushort)num2;
                    }
                    else
                    {
                        scale = ushort.MaxValue;
                        bigInteger /= BigInteger.Pow(10, num2 - ushort.MaxValue);
                    }
                }
            }
            
            return new BigDecimal(bigInteger, scale);
        }
        
        public static BigDecimal operator +(BigDecimal left, BigDecimal right)
        {
            var scale = SameScale(ref left, ref right);
            
            return new BigDecimal(left._value + right._value, scale);
        }

        public static BigDecimal operator -(BigDecimal left, BigDecimal right)
        {
            var scale = SameScale(ref left, ref right);
            
            return new BigDecimal(left._value - right._value, scale);
        }

        public static BigDecimal operator *(BigDecimal left, BigDecimal right)
        {
            var bigInteger = left._value * right._value;
            var num = left._scale + right._scale;
            
            if (num > ushort.MaxValue)
            {
                bigInteger /= BigInteger.Pow(10, num - ushort.MaxValue);
                num = ushort.MaxValue;
            }
            
            return new BigDecimal(bigInteger, (ushort)num);
        }

        public static BigDecimal operator /(BigDecimal a, BigDecimal b)
        {
            BigDecimal outVal = 0;
            
            var maxPrecision = BigInteger.Max(a._scale, b._scale);
            
            if (a._scale < maxPrecision)
            {
                a._scale = (ushort)maxPrecision;
                a._value = a._value * BIPow(10, maxPrecision - a._scale);
            }
            
            if (b._scale < maxPrecision)
            {
                b._scale = (ushort)maxPrecision;
                b._value = b._value * BIPow(10, maxPrecision - b._scale);
            }

            outVal._value = BigInteger.DivRem(a._value, b._value, out var remainder);
            
            while (remainder != 0 && outVal._scale < MaxPrecision)
            {
                while (BigInteger.Abs(remainder) < BigInteger.Abs(b._value))
                {
                    remainder *= 10;
                    outVal._value *= 10;
                    outVal._scale++;
                }
                outVal._value = outVal._value + BigInteger.DivRem(remainder, b._value, out remainder);
            }
            
            return outVal;
        }

        public static explicit operator BigInteger(BigDecimal value)
        {
            return value.ToBigInteger();
        }

        public static implicit operator BigDecimal(sbyte value)
        {
            return new BigDecimal(value);
        }

        public static implicit operator BigDecimal(byte value)
        {
            return new BigDecimal(value);
        }

        public static implicit operator BigDecimal(short value)
        {
            return new BigDecimal(value);
        }

        public static implicit operator BigDecimal(ushort value)
        {
            return new BigDecimal(value);
        }

        public static implicit operator BigDecimal(int value)
        {
            return new BigDecimal(value);
        }

        public static implicit operator BigDecimal(uint value)
        {
            return new BigDecimal(value);
        }

        public static implicit operator BigDecimal(long value)
        {
            return new BigDecimal(value);
        }

        public static implicit operator BigDecimal(ulong value)
        {
            return new BigDecimal(value);
        }

        public static implicit operator BigDecimal(decimal value)
        {
            return new BigDecimal(value);
        }

        public static implicit operator BigDecimal(BigInteger value)
        {
            return new BigDecimal(value);
        }
        
        /// <summary>
        ///    Calculates a number raised to a power both numbers represented as BigIntegers
        /// </summary>
        private static BigInteger BIPow(BigInteger input, BigInteger exp)
        {

            if (exp == 0)
            {
                return 1;
            }
            if (exp == 1)
            {
                return input;
            }
            
            BigInteger outVal = 1;
            
            while (exp != 0)
            {
                if (exp % 2 == 1)
                {
                    outVal *= input;
                }
                exp >>= 1;
                input *= input;
            }

            return outVal;
        }
        
        private static ushort SameScale(ref BigDecimal left, ref BigDecimal right)
        {
            var newScale = Math.Max(left._scale, right._scale);
            
            left = left.Upscale(newScale);
            right = right.Upscale(newScale);
            
            return newScale;
        }

        public enum RoundingMode
        {
            /// <summary> 
            ///    Rounding mode where positive values are rounded towards positive infinity
            ///    and negative values towards negative infinity.
            ///    <br />
            ///    Rule: {@code x.round().abs() >= x.abs()}
            /// </summary>
            RoundUp = 0,
            
            /// <summary>
            ///    Rounding mode where the values are rounded towards zero.
            ///    <br />
            ///    Rule: {@code x.round().abs() &lt;= x.abs()}
            /// </summary>
            RoundDown = 1,
            
            /// <summary>
            ///    Rounding mode to round towards positive infinity. For positive values
            ///    this rounding mode behaves as RoundUp, for negative values as RoundDown.
            ///    <br />
            ///    Rule: {@code x.round() >= x}
            /// </summary>
            RoundCeiling = 2,
            
            /// <summary>
            ///   Rounding mode to round towards negative infinity. For positive values
            ///   this rounding mode behaves as {@link #DOWN}, for negative values as
            ///   {@link #UP}.
            ///   <br />
            ///   Rule: {@code x.round() &lt;= x}
            /// </summary>
            RoundFloor = 3,
            
            /// <summary>
            ///    Rounding mode where values are rounded towards the nearest neighbor. Ties
            ///    are broken by rounding up.
            /// </summary>
            RoundHalfUp = 4,
            
            /// <summary>
            ///    Rounding mode where values are rounded towards the nearest neighbor. Ties
            ///    are broken by rounding down.
            /// </summary>
            RoundHalfDown = 5,
            
            /// <summary>
            ///    Rounding mode where values are rounded towards the nearest neighbor. Ties
            ///    are broken by rounding to the even neighbor.
            /// </summary>
            RoundHalfEven = 6,
            
            /// <summary>
            ///    Rounding mode where the rounding operations throws an ArithmeticException
            ///    for the case that rounding is necessary, i.e. for the case that the value
            ///    cannot be represented exactly.
            /// </summary>
            RoundUnnecessary = 7
        }

        private enum ParseState
        {
            Start,
            Integer,
            Decimal,
            E,
            Exponent
        }
    }
}
