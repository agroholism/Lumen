using System.Collections.Generic;

namespace Lumen.Lang.Expressions {
	public class NativeExpression : Expression {
		public LumenFunc function;

		public NativeExpression(LumenFunc function) {
			this.function = function;
		}

		public Expression Closure(ClosureManager manager) {
			return this;
		}

		public IValue Eval(Scope e) {
			return this.function(e);
		}

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}
