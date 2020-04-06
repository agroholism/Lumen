using System;
using System.Collections.Generic;

namespace Lumen.Lang {
	public class BigNumber : Value {
		internal BigFloat value;

		public IType Type => Prelude.Number;

		public BigNumber(Double value) {
			this.value = value;
		}

		public BigNumber(BigFloat value) {
			this.value = value;
		}

		public static implicit operator BigNumber(Double value) {
			return new BigNumber(value);
		}

		public static implicit operator BigNumber(Int32 value) {
			return new BigNumber(value);
		}

		public static BigNumber operator -(BigNumber one) {
			return new BigNumber(-one.value);
		}

		public Value Clone() {
			return this;
		}

		public override Boolean Equals(Object obj) => obj switch
		{
			Number numb => this.value == numb.value,
			BigNumber num => this.value == num.value,
			_ => false
		};

		public Int32 CompareTo(Object obj) => obj switch
		{
			Number numb => this.value.CompareTo(new BigNumber(numb.value)),
			BigNumber num => this.value.CompareTo(num.value),
			Value value => throw new LumenException(Exceptions.TYPE_ERROR.F(this.Type, value.Type)),
			_ => throw new LumenException(Exceptions.TYPE_ERROR.F(this.Type, obj.GetType()))
		};

		public override Int32 GetHashCode() {
			Int32 hashCode = 1927925191;
			hashCode = hashCode * -1521134295 + this.value.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<IType>.Default.GetHashCode(this.Type);
			return hashCode;
		}

		public override String ToString() {
			return this.value.ToString();
		}

		public String ToString(Scope e) {
			return this.ToString();
		}

		public String ToString(String format, IFormatProvider formatProvider) {
			return this.value.ToString();
		}
	}
}
