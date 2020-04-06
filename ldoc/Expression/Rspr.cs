using Lumen.Lang.Expressions;
using Lumen.Lang;
using System;
using String = System.String;
using System.Collections.Generic;

namespace ldoc {
    [Serializable]
    internal class SpreadE : Expression {
        public Expression Closure(ClosureManager manager) {
            return this;
        }
        internal Expression expression;
        public Int32 i;
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public SpreadE(Expression expression) {
            this.expression = expression;
            if (expression is SpreadE) {
                this.i += ((SpreadE)expression).i + 1;
            }
        }

        public Int32 Get() {
            return this.i;
        }

        public Value Eval(Scope e) {
            return this.expression.Eval(e);
        }
    }
}