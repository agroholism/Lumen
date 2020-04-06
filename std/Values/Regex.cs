using System;
using System.Collections.Generic;

namespace Lumen.Lang {
    public class Regex : Value {
		public System.Text.RegularExpressions.Regex InternalValue { get; set; }
        public IType Type => Prelude.Boolean;

        public Regex(System.Text.RegularExpressions.Regex value) {
            this.InternalValue = value;
        }

        public Int32 CompareTo(Object obj) {
            

            throw new LumenException("expected value of type 'Kernel.Boolean'");
        }

        public override Boolean Equals(Object obj) {
          

            return false;
        }

        public String ToString(Scope e) {
            return this.InternalValue.ToString();
        }

        public override String ToString() {
            return this.ToString(null);
        }

        public override Int32 GetHashCode() {
            Int32 hashCode = 1927925191;
            hashCode = hashCode * -1521134295 + this.InternalValue.GetHashCode();
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
