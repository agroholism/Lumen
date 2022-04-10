using System;
using System.Collections.Generic;

namespace Lumen.Lang.Expressions {
	public class Next : Exception, Expression {
		private String label;

		public Next(String label) : base("unexpected next with label " + label) {
			this.label = label;
		}

		public Boolean IsMatch(String otherLabel) {
			return this.label == otherLabel;
		}

		public IValue Eval(Scope e) {
			throw this;
		}

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public Expression Closure(ClosureManager manager) {
			return this;
		}
	}
}