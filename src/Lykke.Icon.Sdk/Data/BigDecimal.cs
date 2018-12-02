using System.Text;

namespace System.Numerics
{
    public struct BigDecimal
    {
        private BigInteger value;
        private ushort scale;

        static BigInteger maxPrecision = 20;
        public static BigInteger MAXPRECISION
        {
            get { return maxPrecision; }
        }

        public BigDecimal(float value)
        {
            this = BigDecimal.Parse(value.ToString("R"));
        }

        public BigDecimal(double value)
        {
            this = BigDecimal.Parse(value.ToString("R"));
        }

        public BigDecimal(Decimal value)
        {
            this = BigDecimal.Parse(value.ToString());
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
            this = new BigDecimal(value, (ushort)0);
        }

        public BigDecimal(BigInteger value, ushort scale)
        {
            this.value = value;
            this.scale = scale;
        }

        public BigInteger ToBigInteger()
        {
            var scaleDivisor = BigInteger.Pow(new BigInteger(10), this.scale);
            var scaledValue = BigInteger.Divide(this.value, scaleDivisor);
            return scaledValue;
        }

        public static BigDecimal Parse(string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str), "BigDecimal.Parse: Cannot parse null");
            ushort scale = 0;
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder exponentBuilder = (StringBuilder)null;
            BigDecimal.ParseState state = BigDecimal.ParseState.Start;
            Action<char> action1 = (Action<char>)(c =>
            {
                throw new FormatException("BigDecimal.Parse: invalid character '" + (object)c + "' in: " + str);
            });
            Action action2 = (Action)(() =>
            {
                exponentBuilder = new StringBuilder();
                state = BigDecimal.ParseState.E;
            });
            foreach (char c in str)
            {
                switch (state)
                {
                    case BigDecimal.ParseState.Start:
                        if (!char.IsDigit(c))
                        {
                            switch (c)
                            {
                                case '+':
                                case '-':
                                    break;
                                case '.':
                                    state = BigDecimal.ParseState.Decimal;
                                    continue;
                                default:
                                    action1(c);
                                    continue;
                            }
                        }
                        state = BigDecimal.ParseState.Integer;
                        stringBuilder.Append(c);
                        break;
                    case BigDecimal.ParseState.Integer:
                        if (char.IsDigit(c))
                        {
                            stringBuilder.Append(c);
                            break;
                        }
                        switch (c)
                        {
                            case '.':
                                state = BigDecimal.ParseState.Decimal;
                                continue;
                            case 'E':
                            case 'e':
                                action2();
                                continue;
                            default:
                                action1(c);
                                continue;
                        }
                    case BigDecimal.ParseState.Decimal:
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
                    case BigDecimal.ParseState.E:
                        if (char.IsDigit(c) || c == '-' || c == '+')
                        {
                            state = BigDecimal.ParseState.Exponent;
                            exponentBuilder.Append(c);
                            break;
                        }
                        action1(c);
                        break;
                    case BigDecimal.ParseState.Exponent:
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
                throw new FormatException("BigDecimal.Parse: string didn't contain a value: \"" + str + "\"");
            if (exponentBuilder != null && (exponentBuilder.Length == 0 || stringBuilder.Length == 1 && !char.IsDigit(stringBuilder[0])))
                throw new FormatException("BigDecimal.Parse: string contained an 'E' but no exponent value: \"" + str + "\"");
            BigInteger bigInteger = BigInteger.Parse(stringBuilder.ToString());
            if (exponentBuilder != null)
            {
                int num1 = int.Parse(exponentBuilder.ToString());
                if (num1 > 0)
                {
                    if (num1 <= (int)scale)
                    {
                        scale -= (ushort)num1;
                    }
                    else
                    {
                        int exponent = num1 - (int)scale;
                        scale = (ushort)0;
                        bigInteger *= BigInteger.Pow((BigInteger)10, exponent);
                    }
                }
                else if (num1 < 0)
                {
                    int num2 = -num1 + (int)scale;
                    if (num2 <= (int)ushort.MaxValue)
                    {
                        scale = (ushort)num2;
                    }
                    else
                    {
                        scale = ushort.MaxValue;
                        bigInteger /= BigInteger.Pow((BigInteger)10, num2 - (int)ushort.MaxValue);
                    }
                }
            }
            return new BigDecimal(bigInteger, scale);
        }

        private BigDecimal Upscale(ushort newScale)
        {
            if ((int)newScale < (int)this.scale)
                throw new InvalidOperationException("Cannot upscale a BigDecimal to a smaller scale!");
            return new BigDecimal(this.value * BigInteger.Pow((BigInteger)10, (int)newScale - (int)this.scale), newScale);
        }

        private static ushort SameScale(ref BigDecimal left, ref BigDecimal right)
        {
            ushort newScale = Math.Max(left.scale, right.scale);
            left = left.Upscale(newScale);
            right = right.Upscale(newScale);
            return newScale;
        }

        public static BigDecimal operator +(BigDecimal left, BigDecimal right)
        {
            ushort scale = BigDecimal.SameScale(ref left, ref right);
            return new BigDecimal(left.value + right.value, scale);
        }

        public static BigDecimal operator -(BigDecimal left, BigDecimal right)
        {
            ushort scale = BigDecimal.SameScale(ref left, ref right);
            return new BigDecimal(left.value - right.value, scale);
        }

        public static BigDecimal operator *(BigDecimal left, BigDecimal right)
        {
            BigInteger bigInteger = left.value * right.value;
            int num = (int)left.scale + (int)right.scale;
            if (num > (int)ushort.MaxValue)
            {
                bigInteger /= BigInteger.Pow((BigInteger)10, num - (int)ushort.MaxValue);
                num = (int)ushort.MaxValue;
            }
            return new BigDecimal(bigInteger, (ushort)num);
        }

        //Division operator
        public static BigDecimal operator /(BigDecimal a, BigDecimal b)
        {
            BigDecimal outVal = 0;
            BigInteger dividend = a.value;
            BigInteger divisor = b.value;
            BigInteger maxPrecision = BigInteger.Max(a.scale, b.scale);
            if (a.scale < maxPrecision)
            {
                a.scale = (ushort)maxPrecision;
                a.value = a.value * BIPow(10, maxPrecision - a.scale);
            }
            if (b.scale < maxPrecision)
            {
                b.scale = (ushort)maxPrecision;
                b.value = b.value * BIPow(10, maxPrecision - b.scale);
            }
            BigInteger remainder = 0;

            outVal.value = BigInteger.DivRem(a.value, b.value, out remainder);
            while (remainder != 0 && outVal.scale < MAXPRECISION)
            {
                while (BigInteger.Abs(remainder) < BigInteger.Abs(b.value))
                {
                    remainder *= 10;
                    outVal.value *= 10;
                    outVal.scale++;
                }
                outVal.value = outVal.value + BigInteger.DivRem(remainder, b.value, out remainder);
            }
            return outVal;
        }

        //A function to calculate a number raised to a power both numbers represented as BigIntegers
        static BigInteger BIPow(BigInteger input, BigInteger exp)
        {

            if (exp == 0)
            {
                return 1;
            }
            if (exp == 1)
            {
                return input;
            }
            BigInteger outval = 1;
            while (exp != 0)
            {
                if (exp % 2 == 1)
                {
                    outval *= input;
                }
                exp >>= 1;
                input *= input;
            }

            return outval;
        }

        public static explicit operator BigInteger(BigDecimal value)
        {
            return value.ToBigInteger();
        }

        public static implicit operator BigDecimal(sbyte value)
        {
            return new BigDecimal((long)value);
        }

        public static implicit operator BigDecimal(byte value)
        {
            return new BigDecimal((long)value);
        }

        public static implicit operator BigDecimal(short value)
        {
            return new BigDecimal((long)value);
        }

        public static implicit operator BigDecimal(ushort value)
        {
            return new BigDecimal((long)value);
        }

        public static implicit operator BigDecimal(int value)
        {
            return new BigDecimal((long)value);
        }

        public static implicit operator BigDecimal(uint value)
        {
            return new BigDecimal((long)value);
        }

        public static implicit operator BigDecimal(long value)
        {
            return new BigDecimal(value);
        }

        public static implicit operator BigDecimal(ulong value)
        {
            return new BigDecimal(value);
        }

        public static implicit operator BigDecimal(Decimal value)
        {
            return new BigDecimal(value);
        }

        public static implicit operator BigDecimal(BigInteger value)
        {
            return new BigDecimal(value);
        }

        public override string ToString()
        {
            if (this.scale == (ushort)0)
                return this.value.ToString();
            string str = this.value.ToString();
            if (str.Length > (int)this.scale)
                return str.Insert(str.Length - (int)this.scale, ".");
            return "0." + new string('0', (int)this.scale - str.Length) + str;
        }

        

        //Imported from java
        public enum RoundingMode
        {
            /**
             * Rounding mode where positive values are rounded towards positive infinity
             * and negative values towards negative infinity.
             * <br>
             * Rule: {@code x.round().abs() >= x.abs()}
             * 
             * @since Android 1.0
             */
            ROUND_UP = 0,
            /**
             * Rounding mode where the values are rounded towards zero.
             * <br>
             * Rule: {@code x.round().abs() <= x.abs()}
             * 
             * @since Android 1.0
             */
            ROUND_DOWN = 1,
            /**
             * Rounding mode to round towards positive infinity. For positive values
             * this rounding mode behaves as {@link #UP}, for negative values as
             * {@link #DOWN}.
             * <br>
             * Rule: {@code x.round() >= x}
             * 
             * @since Android 1.0
             */
            ROUND_CEILING = 2,
            /**
             * Rounding mode to round towards negative infinity. For positive values
             * this rounding mode behaves as {@link #DOWN}, for negative values as
             * {@link #UP}.
             * <br>
             * Rule: {@code x.round() <= x}
             * 
             * @since Android 1.0
             */
            ROUND_FLOOR = 3,
            /**
             * Rounding mode where values are rounded towards the nearest neighbor. Ties
             * are broken by rounding up.
             * 
             * @since Android 1.0
             */
            ROUND_HALF_UP = 4,
            /**
             * Rounding mode where values are rounded towards the nearest neighbor. Ties
             * are broken by rounding down.
             * 
             * @since Android 1.0
             */
            ROUND_HALF_DOWN = 5,
            /**
             * Rounding mode where values are rounded towards the nearest neighbor. Ties
             * are broken by rounding to the even neighbor.
             * 
             * @since Android 1.0
             */
            ROUND_HALF_EVEN = 6,
            /**
             * Rounding mode where the rounding operations throws an ArithmeticException
             * for the case that rounding is necessary, i.e. for the case that the value
             * cannot be represented exactly.
             * 
             * @since Android 1.0
             */
            ROUND_UNNECESSARY = 7
        }

        private enum ParseState
        {
            Start,
            Integer,
            Decimal,
            E,
            Exponent,
        }
    }
}
