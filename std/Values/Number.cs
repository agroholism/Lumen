using System;
using System.Collections.Generic;

namespace Lumen.Lang {
	public class Number : Value {
		internal Double value;

		public IType Type => Prelude.Number;

		public Number(Double value) {
			this.value = value;
		}

		public static implicit operator Number(Double value) {
			return new Number(value);
		}

		public static implicit operator Number(Int32 value) {
			return new Number(value);
		}

		public override Boolean Equals(Object obj) => obj switch
		{
			Number num => this.value == num.value,
			_ => false
		};

		public Int32 CompareTo(Object obj) => obj switch
		{
			Number num => this.value.CompareTo(num.value),
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
			System.Globalization.NumberFormatInfo n = new System.Globalization.NumberFormatInfo {
				CurrencyGroupSeparator = " ",
				CurrencyDecimalSeparator = ".",
				PercentGroupSeparator = " ",
				NumberGroupSeparator = " "
			};
			return this.value.ToString(n);
		}

		public String ToString(String format, IFormatProvider formatProvider) {
			System.Globalization.NumberFormatInfo n = new System.Globalization.NumberFormatInfo {
				CurrencyGroupSeparator = " ",
				CurrencyDecimalSeparator = ".",
				PercentGroupSeparator = " ",
				NumberGroupSeparator = " "
			};
			return this.value.ToString(format, n);
		}
	}
}
