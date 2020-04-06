using System.Collections.Generic;
using Argent.Xenon.Runtime;

namespace Argent.Xenon.Ast {
	internal class StringE : Expression {
		private System.String text;

		public StringE(System.String text) {
			this.text = text;
		}

		public Expression Closure(ClosureManager manager) {
			return this;
		}

		public XnObject Eval(Scope scope) {
			return new Text(this.text);
		}

		public IEnumerable<XnObject> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}