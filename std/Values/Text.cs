using System;

namespace Lumen.Lang {
	public class Text : IValue {
		private String internalValue;

		public IType Type => Prelude.Text;

		public Text(String value) {
			this.internalValue = value;
		}

		public static explicit operator String(Text value) {
			return value.internalValue;
		}

		public override String ToString() {
			return this.internalValue;
		}

		public override Boolean Equals(Object obj) {
			if (obj is Text String) {
				return this.internalValue == String.internalValue;
			}
			return false;
		}

		public override Int32 GetHashCode() {
			return this.internalValue.GetHashCode();
		}

		public String ToString(String format, IFormatProvider formatProvider) {
			return this.internalValue.ToString(formatProvider);
		}

		public Int32 CompareTo(Object obj) {
			return obj is Text text
				? this.internalValue.CompareTo(text.internalValue)
				: throw new LumenException("can not compare");
		}
	}
}
