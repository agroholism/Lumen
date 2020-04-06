using System.Collections.Generic;
using Argent.Xenon.Runtime;

namespace Argent.Xenon.Ast {
	internal class Yield : Expression {
		private Expression expression;

		public Yield(Expression expression) {
			this.expression = expression;
		}

		public Expression Closure(ClosureManager manager) {
			manager.HasYield = true;
			return new Yield(expression.Closure(manager));
		}

		public XnObject Eval(Scope scope) {
			return this.expression.Eval(scope);
		}

		public IEnumerable<XnObject> EvalWithYield(Scope scope) {
			yield return this.expression.Eval(scope);
		}
	}
}