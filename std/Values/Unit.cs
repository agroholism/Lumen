using System;

namespace Lumen.Lang {
	public class Unit : Value {
		public IType Type => Prelude.Unit;

		public Int32 CompareTo(Object obj) {
			return obj is Unit ? 0 : throw new LumenException("expected value of type 'Kernel.Boolean'");
		}

		public override Boolean Equals(Object obj) {
			return obj is Unit;
		}

		public override String ToString() {
			return "()";
		}

		public String ToString(String format, IFormatProvider formatProvider) {
			return this.ToString();
		}

		public override Int32 GetHashCode() {
			return base.GetHashCode();
		}
	}
}
