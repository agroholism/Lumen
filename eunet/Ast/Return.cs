using System;
using System.Collections.Generic;
using Argent.Xenon.Runtime;

namespace Argent.Xenon.Ast {
	internal class Return : Exception, Expression {
		private Expression expression;
		internal XnObject result;
		
		public Return(Expression expression) {
			this.expression = expression;
		}

		public Expression Closure(ClosureManager manager) {
			return new Return(this.expression.Closure(manager));
		}

		public XnObject Eval(Scope scope) {
			this.result = this.expression.Eval(scope);
			throw this;
		}

		public IEnumerable<XnObject> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}