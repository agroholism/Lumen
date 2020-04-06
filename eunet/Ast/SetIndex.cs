using System.Collections.Generic;
using Argent.Xenon.Runtime;

namespace Argent.Xenon.Ast {
	internal class SetIndex : Expression {
		private Expression sliced;
		private Expression expression;

		public SetIndex(Expression sliced, Expression expression) {
			this.sliced = sliced;
			this.expression = expression;
		}

		public Expression Closure(ClosureManager manager) {
			throw new System.NotImplementedException();
		}

		public XnObject Eval(Scope scope) {
			throw new System.NotImplementedException();
		}

		public IEnumerable<XnObject> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}