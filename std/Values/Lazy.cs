using System;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Lang {

	public class Lazy : Value {
        internal Expressions.Expression expression;
		public Value value;

        public IType Type => this.Force().Type;

        public Lazy(Expressions.Expression expression) {
            this.expression = expression;
        }

        public Int32 CompareTo(Object obj) {
			return this.Force().CompareTo(obj);
        }

		public Value Force(Scope scope=null) {
			if (this.value == null) {
				this.value = this.expression.Eval(scope ?? new Scope());
			}

			return this.value;
		}

        public override Boolean Equals(Object obj) {
            return this.Force(null).Equals(obj);
        }

        public String ToString(Scope e) {
            return this.Force(e).ToString();
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
