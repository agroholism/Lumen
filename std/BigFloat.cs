using System;
using System.Linq;
using System.Text;
using System.Numerics;
namespace Lumen.Lang.Std {
	[Serializable]
	public class BigFloat : IComparable, IComparable<BigFloat>, IEquatable<BigFloat> {
		private BigInteger numerator;
		private BigInteger denominator;

		public static readonly BigFloat One = new BigFloat(1);
		public static readonly BigFloat Zero = new BigFloat(0);
		public static readonly BigFloat MinusOne = new BigFloat(-1);
		public static readonly BigFloat OneHalf = new BigFloat(1, 2);

		public Int32 Sign {
			get {
				switch (this.numerator.Sign + this.denominator.Sign) {
					case 2:
					case -2:
						return 1;
					case 0:
						return -1;
					default:
						return 0;
				}
			}
		}

		public Boolean IsPositiveInfinity {
			get => this.denominator.IsZero && this.numerator > 0;
		}

		public Boolean IsNeagtiveInfinity {
			get => this.denominator.IsZero && this.numerator < 0;
		}

		public Boolean IsNaN {
			get => this.denominator.IsZero && this.numerator == 0;
		}

		//constructors
		public BigFloat() {
			this.numerator = BigInteger.Zero;
			this.denominator = BigInteger.One;
		}

		public BigFloat(String value) {
			BigFloat bf = Parse(value);
			this.numerator = bf.numerator;
			this.denominator = bf.denominator;
		}

		public BigFloat(BigInteger numerator, BigInteger denominator) {
			this.numerator = numerator;
			this.denominator = BigInteger.Abs(denominator);
		}

		public BigFloat(BigInteger value) {
			this.numerator = value;
			this.denominator = BigInteger.One;
		}

		public BigFloat(BigFloat value) {
			if (BigFloat.Equals(value, null)) {
				this.numerator = BigInteger.Zero;
				this.denominator = BigInteger.One;
			}
			else {

				this.numerator = value.numerator;
				this.denominator = value.denominator;
			}
		}

		public BigFloat(UInt64 value) {
			this.numerator = new BigInteger(value);
			this.denominator = BigInteger.One;
		}

		public BigFloat(Int64 value) {
			this.numerator = new BigInteger(value);
			this.denominator = BigInteger.One;
		}

		public BigFloat(UInt32 value) {
			this.numerator = new BigInteger(value);
			this.denominator = BigInteger.One;
		}

		public BigFloat(Int32 value) {
			this.numerator = new BigInteger(value);
			this.denominator = BigInteger.One;
		}

		public BigFloat(Single value) : this(value.ToString("N99", System.Globalization.CultureInfo.InvariantCulture.NumberFormat)) {
		}

		public BigFloat(Double value) : this(value.ToString("N99", System.Globalization.CultureInfo.InvariantCulture.NumberFormat)) {
			
		}

		public BigFloat(Decimal value) : this(value.ToString("N99", System.Globalization.CultureInfo.InvariantCulture.NumberFormat)) {
		}

		//non-static methods
		public BigFloat Add(BigFloat other) {
			if (BigFloat.Equals(other, null)) {
				throw new ArgumentNullException("other");
			}

			this.numerator = this.numerator * other.denominator + other.numerator * this.denominator;
			this.denominator *= other.denominator;
			return this;
		}

		public BigFloat Subtract(BigFloat other) {
			if (BigFloat.Equals(other, null)) {
				throw new ArgumentNullException("other");
			}

			this.numerator = this.numerator * other.denominator - other.numerator * this.denominator;
			this.denominator *= other.denominator;
			return this;
		}

		public BigFloat Multiply(BigFloat other) {
			if (Equals(other, null)) {
				throw new ArgumentNullException("other");
			}

			this.numerator *= other.numerator;
			this.denominator *= other.denominator;
			return this;
		}

		public BigFloat Divide(BigFloat other) {
			if (BigInteger.Equals(other, null)) {
				throw new ArgumentNullException("other");
			}

			this.numerator *= other.denominator;
			this.denominator *= other.numerator;
			return this;
		}

		public BigFloat Remainder(BigFloat other) {
			if (BigInteger.Equals(other, null)) {
				throw new ArgumentNullException("other");
			}

			//b = a mod n
			//remainder = a - floor(a/n) * n

			BigFloat result = this - Floor(this / other) * other;

			this.numerator = result.numerator;
			this.denominator = result.denominator;


			return this;
		}

		public BigFloat DivideRemainder(BigFloat other, out BigFloat remainder) {
			this.Divide(other);

			remainder = BigFloat.Remainder(this, other);

			return this;
		}

		public BigFloat Pow(Int32 exponent) {
			if (this.numerator.IsZero) {
				// Nothing to do
			}
			else if (exponent < 0) {
				BigInteger savedNumerator = this.numerator;
				this.numerator = BigInteger.Pow(this.denominator, -exponent);
				this.denominator = BigInteger.Pow(savedNumerator, -exponent);
			}
			else {
				this.numerator = BigInteger.Pow(this.numerator, exponent);
				this.denominator = BigInteger.Pow(this.denominator, exponent);
			}

			return this;
		}

		public BigFloat Abs() {
			this.numerator = BigInteger.Abs(this.numerator);
			return this;
		}

		public BigFloat Negate() {
			this.numerator = BigInteger.Negate(this.numerator);
			return this;
		}

		public BigFloat Inverse() {
			BigInteger temp = this.numerator;
			this.numerator = this.denominator;
			this.denominator = temp;
			return this;
		}

		public BigFloat Increment() {
			this.numerator += this.denominator;
			return this;
		}

		public BigFloat Decrement() {
			this.numerator -= this.denominator;
			return this;
		}

		public BigFloat Ceil() {
			if (this.numerator < 0) {
				this.numerator -= BigInteger.Remainder(this.numerator, this.denominator);
			}
			else {
				this.numerator += this.denominator - BigInteger.Remainder(this.numerator, this.denominator);
			}

			Factor();
			return this;
		}

		public BigFloat Floor() {
			if (this.numerator < 0) {
				this.numerator += this.denominator - BigInteger.Remainder(this.numerator, this.denominator);
			}
			else {
				this.numerator -= BigInteger.Remainder(this.numerator, this.denominator);
			}

			Factor();
			return this;
		}

		public BigFloat Round() {
			//get remainder. Over divisor see if it is > new BigFloat(0.5)
			BigFloat value = BigFloat.Decimals(this);

			if (value.CompareTo(OneHalf) >= 0) {
				this.Ceil();
			}
			else {
				this.Floor();
			}

			return this;
		}

		public BigFloat Truncate() {
			this.numerator -= BigInteger.Remainder(this.numerator, this.denominator);
			Factor();
			return this;
		}

		public BigFloat Decimals() {
			BigInteger result = BigInteger.Remainder(this.numerator, this.denominator);

			return new BigFloat(result, this.denominator);
		}

		public BigFloat ShiftDecimalLeft(Int32 shift) {
			if (shift < 0) {
				return ShiftDecimalRight(-shift);
			}

			this.numerator *= BigInteger.Pow(10, shift);
			return this;
		}

		public BigFloat ShiftDecimalRight(Int32 shift) {
			if (shift < 0) {
				return ShiftDecimalLeft(-shift);
			}

			this.denominator *= BigInteger.Pow(10, shift);
			return this;
		}

		public Double Sqrt() {
			return Math.Pow(10, BigInteger.Log10(this.numerator) / 2) / Math.Pow(10, BigInteger.Log10(this.denominator) / 2);
		}

		public Double Log10() {
			return BigInteger.Log10(this.numerator) - BigInteger.Log10(this.denominator);
		}

		public Double Log(Double baseValue) {
			return BigInteger.Log(this.numerator, baseValue) - BigInteger.Log(this.numerator, baseValue);
		}

		public override String ToString() {
			//default precision = 100
			return ToString(50);
		}

		public String ToString(Int32 precision, Boolean trailingZeros = false) {
			if(this.denominator == 0) {
				if(this.numerator > 0) {
					return Double.PositiveInfinity.ToString();
				}
				else if(this.numerator < 0) {
					return Double.NegativeInfinity.ToString();
				}
				return Double.NaN.ToString();
			}

			Factor();

			BigInteger result = BigInteger.DivRem(this.numerator, this.denominator, out BigInteger remainder);

			if (remainder == 0 && !trailingZeros) {
				return result + ".0";
			}
			else if (remainder == 0) {
				return result.ToString();
			}

			BigInteger decimals = (this.numerator * BigInteger.Pow(10, precision)) / this.denominator;

			if (decimals == 0 && !trailingZeros) {
				return result + ".0";
			}
			else if (decimals == 0) {
				return result.ToString();
			}

			decimals = BigInteger.Abs(decimals); 

			StringBuilder sb = new StringBuilder();

			while (precision-- > 0) {
				sb.Append(decimals % 10);
				decimals /= 10;
			}

			if (trailingZeros) {
				return result + "." + new String(sb.ToString().Reverse().ToArray());
			}
			else {
				return result + "." + new String(sb.ToString().Reverse().ToArray()).TrimEnd(new Char[] { '0' });
			}
		}

		public String ToMixString() {
			if (this.denominator == 0) {
				if (this.numerator > 0) {
					return Double.PositiveInfinity.ToString();
				}
				else if (this.numerator < 0) {
					return Double.NegativeInfinity.ToString();
				}
				return Double.NaN.ToString();
			}

			Factor();

			BigInteger result = BigInteger.DivRem(this.numerator, this.denominator, out BigInteger remainder);

			if (remainder == 0) {
				return result.ToString();
			}
			else {
				return result + ", " + remainder + "/" + this.denominator;
			}
		}

		public String ToRationalString() {
			if (this.denominator == 0) {
				if (this.numerator > 0) {
					return Double.PositiveInfinity.ToString();
				}
				else if (this.numerator < 0) {
					return Double.NegativeInfinity.ToString();
				}
				return Double.NaN.ToString();
			}

			Factor();

			return this.numerator + "/" + this.denominator;
		}

		public Int32 CompareTo(BigFloat other) {
			if (BigFloat.Equals(other, null)) {
				throw new ArgumentNullException("other");
			}

			//Make copies
			BigInteger one = this.numerator;
			BigInteger two = other.numerator;

			//cross multiply
			one *= other.denominator;
			two *= this.denominator;

			//test
			return BigInteger.Compare(one, two);
		}

		public Int32 CompareTo(Object other) {
			if (other == null) {
				throw new ArgumentNullException("other");
			}

			if (!(other is BigFloat)) {
				throw new System.ArgumentException("other is not a BigFloat");
			}

			return CompareTo((BigFloat)other);
		}

		public override Boolean Equals(Object other) {
			if (other == null || GetType() != other.GetType()) {
				return false;
			}

			return this.numerator == ((BigFloat)other).numerator && this.denominator == ((BigFloat)other).denominator;
		}

		public Boolean Equals(BigFloat other) {
			return (other.numerator == this.numerator && other.denominator == this.denominator);
		}

		public override Int32 GetHashCode() {
			return this.numerator.GetHashCode() | this.denominator.GetHashCode();
			//return base.GetHashCode();
		}

		//static methods
		public new static Boolean Equals(Object left, Object right) {
			if (left == null && right == null) {
				return true;
			}
			else if (left == null || right == null) {
				return false;
			}
			else if (left.GetType() != right.GetType()) {
				return false;
			}
			else {
				return (((BigInteger)left).Equals((BigInteger)right));
			}
		}

		public static String ToString(BigFloat value) {
			return value.ToString();
		}

		public static BigFloat Inverse(BigFloat value) {
			return (new BigFloat(value)).Inverse();
		}

		public static BigFloat Decrement(BigFloat value) {
			return (new BigFloat(value)).Decrement();
		}

		public static BigFloat Negate(BigFloat value) {
			return (new BigFloat(value)).Negate();
		}

		public static BigFloat Increment(BigFloat value) {
			return (new BigFloat(value)).Increment();
		}

		public static BigFloat Abs(BigFloat value) {
			return (new BigFloat(value)).Abs();
		}

		public static BigFloat Add(BigFloat left, BigFloat right) {
			return (new BigFloat(left)).Add(right);
		}

		public static BigFloat Subtract(BigFloat left, BigFloat right) {
			return (new BigFloat(left)).Subtract(right);
		}

		public static BigFloat Multiply(BigFloat left, BigFloat right) {
			return (new BigFloat(left)).Multiply(right);
		}

		public static BigFloat Divide(BigFloat left, BigFloat right) {
			return (new BigFloat(left)).Divide(right);
		}

		public static BigFloat Pow(BigFloat value, Int32 exponent) {
			return (new BigFloat(value)).Pow(exponent);
		}

		public static BigFloat Remainder(BigFloat left, BigFloat right) {
			return (new BigFloat(left)).Remainder(right);
		}

		public static BigFloat DivideRemainder(BigFloat left, BigFloat right, out BigFloat remainder) {
			return (new BigFloat(left)).DivideRemainder(right, out remainder);
		}

		public static BigFloat Decimals(BigFloat value) {
			return value.Decimals();
		}

		public static BigFloat Truncate(BigFloat value) {
			return (new BigFloat(value)).Truncate();
		}

		public static BigFloat Ceil(BigFloat value) {
			return (new BigFloat(value)).Ceil();
		}

		public static BigFloat Floor(BigFloat value) {
			return (new BigFloat(value)).Floor();
		}

		public static BigFloat Round(BigFloat value) {
			return (new BigFloat(value)).Round();
		}

		public static BigFloat Parse(String value) {
			if (value == null) {
				throw new ArgumentNullException("value");
			}

			value.Trim();
			value = value.Replace(",", "");
			Int32 pos = value.IndexOf('.');
			value = value.Replace(".", "");

			if (pos < 0) {
				//no decimal point
				BigInteger numerator = BigInteger.Parse(value);
				return (new BigFloat(numerator)).Factor();
			}
			else {
				//decimal point (length - pos - 1)
				BigInteger numerator = BigInteger.Parse(value);
				BigInteger denominator = BigInteger.Pow(10, value.Length - pos);

				return (new BigFloat(numerator, denominator)).Factor();
			}
		}

		public static BigFloat ShiftDecimalLeft(BigFloat value, Int32 shift) {
			return (new BigFloat(value)).ShiftDecimalLeft(shift);
		}

		public static BigFloat ShiftDecimalRight(BigFloat value, Int32 shift) {
			return (new BigFloat(value)).ShiftDecimalRight(shift);
		}
		public static Boolean TryParse(String value, out BigFloat result) {
			try {
				result = BigFloat.Parse(value);
				return true;
			}
			catch (ArgumentNullException) {
				result = null;
				return false;
			}
			catch (FormatException) {
				result = null;
				return false;
			}
		}
		public static Int32 Compare(BigFloat left, BigFloat right) {
			if (BigFloat.Equals(left, null)) {
				throw new ArgumentNullException("left");
			}

			if (BigFloat.Equals(right, null)) {
				throw new ArgumentNullException("right");
			}

			return (new BigFloat(left)).CompareTo(right);
		}
		public static Double Log10(BigFloat value) {
			return (new BigFloat(value)).Log10();
		}
		public static Double Log(BigFloat value, Double baseValue) {
			return (new BigFloat(value)).Log(baseValue);
		}
		public static Double Sqrt(BigFloat value) {
			return (new BigFloat(value)).Sqrt();
		}

		public static BigFloat operator -(BigFloat value) {
			return (new BigFloat(value)).Negate();
		}
		public static BigFloat operator -(BigFloat left, BigFloat right) {
			return (new BigFloat(left)).Subtract(right);
		}
		public static BigFloat operator --(BigFloat value) {
			return value.Decrement();
		}
		public static BigFloat operator +(BigFloat left, BigFloat right) {
			return (new BigFloat(left)).Add(right);
		}
		public static BigFloat operator +(BigFloat value) {
			return (new BigFloat(value)).Abs();
		}
		public static BigFloat operator ++(BigFloat value) {
			return value.Increment();
		}
		public static BigFloat operator %(BigFloat left, BigFloat right) {
			return (new BigFloat(left)).Remainder(right);
		}
		public static BigFloat operator *(BigFloat left, BigFloat right) {
			return (new BigFloat(left)).Multiply(right);
		}
		public static BigFloat operator /(BigFloat left, BigFloat right) {
			return (new BigFloat(left)).Divide(right);
		}
		public static BigFloat operator >>(BigFloat value, Int32 shift) {
			return (new BigFloat(value)).ShiftDecimalRight(shift);
		}
		public static BigFloat operator <<(BigFloat value, Int32 shift) {
			return (new BigFloat(value)).ShiftDecimalLeft(shift);
		}
		public static BigFloat operator ^(BigFloat left, Int32 right) {
			return (new BigFloat(left)).Pow(right);
		}
		public static BigFloat operator ~(BigFloat value) {
			return (new BigFloat(value)).Inverse();
		}

		public static Boolean operator !=(BigFloat left, BigFloat right) {
			return Compare(left, right) != 0;
		}
		public static Boolean operator ==(BigFloat left, BigFloat right) {
			return Compare(left, right) == 0;
		}
		public static Boolean operator <(BigFloat left, BigFloat right) {
			return Compare(left, right) < 0;
		}
		public static Boolean operator <=(BigFloat left, BigFloat right) {
			return Compare(left, right) <= 0;
		}
		public static Boolean operator >(BigFloat left, BigFloat right) {
			return Compare(left, right) > 0;
		}
		public static Boolean operator >=(BigFloat left, BigFloat right) {
			return Compare(left, right) >= 0;
		}

		public static Boolean operator true(BigFloat value) {
			return value != 0;
		}
		public static Boolean operator false(BigFloat value) {
			return value == 0;
		}

		public static explicit operator Decimal(BigFloat value) {
			if (Decimal.MinValue > value) {
				throw new OverflowException("value is less than System.decimal.MinValue.");
			}

			if (Decimal.MaxValue < value) {
				throw new OverflowException("value is greater than System.decimal.MaxValue.");
			}

			return (Decimal)value.numerator / (Decimal)value.denominator;
		}
		public static explicit operator Double(BigFloat value) {
			if (Double.MinValue > value) {
				throw new OverflowException("value is less than System.double.MinValue.");
			}

			if (Double.MaxValue < value) {
				throw new OverflowException("value is greater than System.double.MaxValue.");
			}

			return (Double)value.numerator / (Double)value.denominator;
		}
		public static explicit operator Single(BigFloat value) {
			if (Single.MinValue > value) {
				throw new OverflowException("value is less than System.Single.MinValue.");
			}

			if (Single.MaxValue < value) {
				throw new OverflowException("value is greater than System.Single.MaxValue.");
			}

			return (Single)value.numerator / (Single)value.denominator;
		}

		//byte, sbyte, 
		public static implicit operator BigFloat(Byte value) {
			return new BigFloat((UInt32)value);
		}
		public static implicit operator BigFloat(SByte value) {
			return new BigFloat(value);
		}
		public static implicit operator BigFloat(Int16 value) {
			return new BigFloat(value);
		}
		public static implicit operator BigFloat(UInt16 value) {
			return new BigFloat((UInt32)value);
		}
		public static implicit operator BigFloat(Int32 value) {
			return new BigFloat(value);
		}
		public static implicit operator BigFloat(Int64 value) {
			return new BigFloat(value);
		}
		public static implicit operator BigFloat(UInt32 value) {
			return new BigFloat(value);
		}
		public static implicit operator BigFloat(UInt64 value) {
			return new BigFloat(value);
		}
		public static implicit operator BigFloat(Decimal value) {
			return new BigFloat(value);
		}
		public static implicit operator BigFloat(Double value) {
			return new BigFloat(value);
		}
		public static implicit operator BigFloat(Single value) {
			return new BigFloat(value);
		}
		public static implicit operator BigFloat(BigInteger value) {
			return new BigFloat(value);
		}

		private BigFloat Factor() {
			//factoring can be very slow. So use only when neccessary (ToString, and comparisons)

			if (this.denominator == 1) {
				return this;
			}

			//factor numerator and denominator
			BigInteger factor = BigInteger.GreatestCommonDivisor(this.numerator, this.denominator);

			this.numerator /= factor;
			this.denominator /= factor;

			return this;
		}

	}
}