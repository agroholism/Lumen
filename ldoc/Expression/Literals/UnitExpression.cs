using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace ldoc {
    public class UnitExpression : Expression {
        public static UnitExpression Instance { get; } = new UnitExpression();

        private UnitExpression() {

        }
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public Expression Closure(ClosureManager manager) {
            return this;
        }

        public Value Eval(Scope scope) {
            return Const.UNIT;
        }

        public override String ToString() {
            return "()";
        }
    }
}