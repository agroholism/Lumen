using System;

namespace Lumen.Lang.Std {
	public class BigNum : Value {
		internal BigFloat value;

		public Record Type {
			get => StandartModule.Number;
		}

		public BigNum(BigFloat value) {
			this.value = value;
		}

		public static implicit operator BigNum(Double value) {
			return new BigNum(value);
		}

		public static implicit operator BigNum(Int32 value) {
			return new BigNum(value);
		}

		public static BigNum operator -(BigNum one) {
			return new BigNum(-one.value);
		}

		public static BigNum operator +(BigNum one, BigNum other) {
			return new BigNum(one.value + other.value);
		}

		public static BigNum operator +(BigNum one, Double other) {
			return new BigNum(one.value + other);
		}

		public static BigNum operator -(BigNum one, BigNum other) {
			return new BigNum(one.value - other.value);
		}

		public static BigNum operator -(BigNum one, Double other) {
			return new BigNum(one.value - other);
		}

		public static BigNum operator /(BigNum one, BigNum other) {
			return new BigNum(one.value / other.value);
		}

		public static BigNum operator /(BigNum one, Double other) {
			return new BigNum(one.value / other);
		}

		public static BigNum operator *(BigNum one, BigNum other) {
			return new BigNum(one.value * other.value);
		}

		public static BigNum operator *(BigNum one, Double other) {
			return new BigNum(one.value * other);
		}

		public static Boolean operator ==(BigNum one, BigNum other) {
			return one.value == other.value;
		}

		public static Boolean operator ==(BigNum one, Double other) {
			return one.value == other;
		}

		public static Boolean operator !=(BigNum one, BigNum other) {
			return one.value != other.value;
		}

		public static Boolean operator !=(BigNum one, Double other) {
			return one.value != other;
		}

		public static Boolean operator >(BigNum one, BigNum other) {
			return one.value > other.value;
		}

		public static Boolean operator >(BigNum one, Double other) {
			return one.value > other;
		}

		public static Boolean operator <(BigNum one, BigNum other) {
			return one.value < other.value;
		}

		public static Boolean operator <(BigNum one, Double other) {
			return one.value < other;
		}

		public static Boolean operator >=(BigNum one, BigNum other) {
			return one.value >= other.value;
		}

		public static Boolean operator >=(BigNum one, Double other) {
			return one.value >= other;
		}

		public static Boolean operator <=(BigNum one, BigNum other) {
			return one.value <= other.value;
		}

		public static Boolean operator <=(BigNum one, Double other) {
			return one.value <= other;
		}

		public Value Clone() {
			return new BigNum(this.value);
		}

		public String ToString(Scope e) {
			return this.value.ToString();
		}

		public override String ToString() {
			return this.ToString(null);
		}

		public override Boolean Equals(Object obj) {
			if (obj is Num num) {
				return this.value == num.value;
			}

			return false;
		}

		public override Int32 GetHashCode() {
			return this.value.GetHashCode();
		}

		public Int32 CompareTo(Object obj) {
			if (obj is Num num) {
				return this.value.CompareTo(num.value);
			}

			throw new Exception("expected value of type 'Kernel.Number'", stack: null);
		}
	}
}
