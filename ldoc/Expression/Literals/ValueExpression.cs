using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;
using String = System.String;

namespace ldoc {
    [Serializable]
    public class ValueE : Expression {
        public Value val;
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public ValueE(Value Object) {
            this.val = Object;
        }

        public Expression Closure(ClosureManager manager) {
            return this;
        }

        public ValueE(Double Object) {
            this.val = new Number(Object);
        }

        public Value Eval(Scope e) {
            return this.val;
        }

        public override String ToString() {
            return this.val.ToString();
        }
    }
}
