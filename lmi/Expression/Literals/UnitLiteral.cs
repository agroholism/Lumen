using System;

using Lumen.Lang.Expressions;
using Lumen.Lang;
using System.Collections.Generic;

namespace Lumen.Lmi {
	public class UnitLiteral : Expression {
        public static UnitLiteral Instance { get; } = new UnitLiteral();

        private UnitLiteral() {

        }

        public Expression Closure(ClosureManager manager) {
            return this;
        }

        public Value Eval(Scope scope) {
            return Const.UNIT;
        }

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public override String ToString() {
            return "()";
        }
    }
}