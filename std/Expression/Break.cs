using System;
using System.Collections.Generic;

namespace Lumen.Lang.Expressions {
	public class Break : Exception, Expression {
		private Int32 label;

		public Break(Int32 label) : base("unexpected break with label") {
			this.label = label;
		}

		public Int32 UseLabel() {
			this.label--;
			return this.label;
		}

		public Value Eval(Scope e) {
			throw this;
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public Expression Closure(ClosureManager manager) {
			return this;
		}
	}
}