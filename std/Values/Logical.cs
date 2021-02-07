using System;
using System.Collections.Generic;

namespace Lumen.Lang {
	public struct Logical : Value {
		private Boolean internalValue;
		public IType Type => Prelude.Logical;

		public Logical(Boolean value) {
			this.internalValue = value;
		}

		public static implicit operator Boolean(Logical value) {
			return value.internalValue;
		}

		public Int32 CompareTo(Object obj) {
			return obj is Logical logical
				? this.internalValue.CompareTo(logical.internalValue)
				: throw new LumenException("expected value of type 'Kernel.Boolean'");
		}

		public override Boolean Equals(Object obj) {
			return obj is Logical logical && this.internalValue == logical.internalValue;
		}

		public override Int32 GetHashCode() {
			Int32 hashCode = 1927925191;
			hashCode = hashCode * -1521134295 + this.internalValue.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<IType>.Default.GetHashCode(this.Type);
			return hashCode;
		}

		public override String ToString() {
			return this.internalValue ? "true" : "false";
		}

		public String ToString(String format, IFormatProvider formatProvider) {
			if (this.Type.HasImplementation(Prelude.Format) &&
				this.Type.GetMember("format").TryConvertToFunction(out Fun function)) {
				function.Call(new Scope(), this, new Text(format ?? "")).ToString();
			}

			return this.ToString();
		}
	}
}
