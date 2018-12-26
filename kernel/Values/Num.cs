using System;

namespace Lumen.Lang.Std {
	public class Num : Value {
		internal BigFloat value;

		public KType Type {
			get => StandartModule.Number;
		}

		public Num(BigFloat value) {
			this.value = value;
		}

		public static implicit operator Num(Double value) {
			return new Num(value);
		}

		public static implicit operator Num(Int32 value) {
			return new Num(value);
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
				var z = this.value.CompareTo(num.value);
				return z;
			}

			throw new Exception("expected value of type 'Kernel.Number'", stack: null);
		}
	}
}
