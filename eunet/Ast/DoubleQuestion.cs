using System.Collections.Generic;
using Argent.Xenon.Runtime;

namespace Argent.Xenon.Ast {
	internal class DoubleQuestion : Expression {
		private Expression result;
		private Expression expression;

		public DoubleQuestion(Expression result, Expression expression) {
			this.result = result;
			this.expression = expression;
		}

		public Expression Closure(ClosureManager manager) {
			return new DoubleQuestion(this.result.Closure(manager), this.expression.Closure(manager));
		}

		public XnObject Eval(Scope scope) {
			XnObject fst = this.result.Eval(scope);
			if(!(fst is Nil)) {
				return fst;
			}

			return this.expression.Eval(scope);
		}

		public IEnumerable<XnObject> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}