using System.Collections.Generic;
using Argent.Xenon.Runtime;
using System.Linq;

namespace Argent.Xenon.Ast {
	internal class YieldSequence : Expression {
		private Expression expression;

		public YieldSequence(Expression expression) {
			this.expression = expression;
		}

		public Expression Closure(ClosureManager manager) {
			manager.HasYield = true;
			return new YieldSequence(this.expression.Closure(manager));
		}

		public XnObject Eval(Scope scope) {
			return this.expression.Eval(scope);
		}

		public IEnumerable<XnObject> EvalWithYield(Scope scope) {
			IEnumerable<XnObject> obj = XnStd.AsSequence(this.expression.Eval(scope));
			return from XnObject i in obj
				   select i;
		}
	}
}