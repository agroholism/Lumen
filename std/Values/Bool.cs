using System;
using System.Collections.Generic;

namespace Lumen.Lang {
    public struct Bool : Value {
        internal Boolean value;
        public IType Type => Prelude.Boolean;

        public Bool(Boolean value) {
            this.value = value;
        }

        public static implicit operator Bool(Boolean value) {
            return new Bool(value);
        }

        public Int32 CompareTo(Object obj) {
            if (obj is Bool b) {
                Boolean value = this.value;
                Boolean other = b.value;

                if (value == other) {
                    return 0;
                } else if (value == false && other == true) {
                    return -1;
                } else {
                    return 1;
                }
            }

            throw new LumenException("expected value of type 'Kernel.Boolean'");
        }

        public override Boolean Equals(Object obj) {
            if(obj is Bool b) {
                return this.value == b.value;
            } 

            return false;
        }

        public String ToString(Scope e) {
            return this.value ? "true" : "false";
        }

        public override String ToString() {
            return this.ToString(null);
        }

        public override Int32 GetHashCode() {
            Int32 hashCode = 1927925191;
            hashCode = hashCode * -1521134295 + this.value.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<IType>.Default.GetHashCode(this.Type);
            return hashCode;
        }

		public String ToString(String format, IFormatProvider formatProvider) {
			return this.ToString(null);
		}

		public Value Clone() {
			throw new NotImplementedException();
		}
	}
}
