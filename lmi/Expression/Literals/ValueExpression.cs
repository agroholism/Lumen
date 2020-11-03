using System;
using System.Collections.Generic;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	public class ValueLiteral : Expression {
        public Value result;

        public ValueLiteral(Value value) {
            this.result = value;
        }

        public ValueLiteral(Double Object) {
            this.result = new Number(Object);
        }

		public Value Eval(Scope e) {
            return this.result;
        }

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			yield return new GeneratorTerminalResult(this.Eval(scope));
		}

		public Expression Closure(ClosureManager manager) {
			return this;
		}

		public override String ToString() {
            return this.result.ToString();
        }
    }
}
