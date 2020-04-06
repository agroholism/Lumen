using System;
using System.Collections.Generic;

namespace Lumen.Lang {
    public struct Unit : Value {
        internal Boolean value;
        public IType Type => Prelude.Unit;

        public Int32 CompareTo(Object obj) {
            if (obj is Unit) {
				return 0;
            }

            throw new LumenException("expected value of type 'Kernel.Boolean'");
        }

        public override Boolean Equals(Object obj) {
			if (obj is Unit) {
				return true;
			}

			return false;
        }

        public override String ToString() {
            return "()";
        }

        public override Int32 GetHashCode() {
            Int32 hashCode = 1927925191;
            hashCode = hashCode * -1521134295 + this.value.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<IType>.Default.GetHashCode(this.Type);
            return hashCode;
        }

		public String ToString(String format, IFormatProvider formatProvider) {
			return this.ToString();
		}

		public Value Clone() {
			return this;
		}
	}
}
