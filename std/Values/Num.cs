using System;

namespace Lumen.Lang.Std {
	public class Num : Value {
		internal Double value;

		public Record Type {
			get => StandartModule.Number;
		}

		public Num(Double value) {
			this.value = value;
		}

		public static implicit operator Num(Double value) {
			return new Num(value);
		}

		public static implicit operator Num(Int32 value) {
			return new Num(value);
		}

		public static Num operator -(Num one) {
			return new Num(-one.value);
		}

		public static Num operator +(Num one,Num other) {
			return new Num(one.value + other.value);
		}

		public static Num operator +(Num one, Double other) {
			return new Num(one.value + other);
		}

		public static Num operator -(Num one, Num other) {
			return new Num(one.value - other.value);
		}

		public static Num operator -(Num one, Double other) {
			return new Num(one.value - other);
		}

		public static Num operator /(Num one, Num other) {
			return new Num(one.value / other.value);
		}

		public static Num operator /(Num one, Double other) {
			return new Num(one.value / other);
		}

		public static Num operator *(Num one, Num other) {
			return new Num(one.value * other.value);
		}

		public static Num operator *(Num one, Double other) {
			return new Num(one.value * other);
		}

		public static Boolean operator ==(Num one, Num other) {
			return one.value == other.value;
		}

		public static Boolean operator ==(Num one, Double other) {
			return one.value == other;
		}

		public static Boolean operator !=(Num one, Num other) {
			return one.value != other.value;
		}

		public static Boolean operator !=(Num one, Double other) {
			return one.value != other;
		}

		public static Boolean operator >(Num one, Num other) {
			return one.value > other.value;
		}

		public static Boolean operator >(Num one, Double other) {
			return one.value > other;
		}

		public static Boolean operator <(Num one, Num other) {
			return one.value < other.value;
		}

		public static Boolean operator <(Num one, Double other) {
			return one.value < other;
		}

		public static Boolean operator >=(Num one, Num other) {
			return one.value >= other.value;
		}

		public static Boolean operator >=(Num one, Double other) {
			return one.value >= other;
		}

		public static Boolean operator <=(Num one, Num other) {
			return one.value <= other.value;
		}

		public static Boolean operator <=(Num one, Double other) {
			return one.value <= other;
		}

		public Value Clone() {
			return new Num(this.value);
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
