using System;
using System.Collections.Generic;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	public class ValueLiteral : Expression {
        public IValue result;

        public ValueLiteral(IValue value) {
            this.result = value;
        }

        public ValueLiteral(Double Object) {
            this.result = new Number(Object);
        }

		public IValue Eval(Scope e) {
            return this.result;
        }

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			yield return new GeneratorExpressionTerminalResult(this.Eval(scope));
		}

		public Expression Closure(ClosureManager manager) {
			return this;
		}

		public override String ToString() {
            return this.result.ToString();
        }
    }
}
