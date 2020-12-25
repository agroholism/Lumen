using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class YieldFrom : Expression {
		private Expression expression;

		public YieldFrom(Expression expression) {
			this.expression = expression;
		}

		public Expression Closure(ClosureManager manager) {
			manager.HasYield = true;
			return new YieldFrom(this.expression.Closure(manager));
		}

		public Value Eval(Scope e) {
			return this.expression.Eval(e);
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			foreach(Value i in this.expression.Eval(scope).ToSeq(scope)) {
				yield return i;
			}
		}
	}
}