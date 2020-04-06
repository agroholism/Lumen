using System.Collections.Generic;
using Argent.Xenon.Runtime;

namespace Argent.Xenon.Ast {
	internal class ValueE : Expression {
		private XnObject euObject;

		public ValueE(System.Double v) {
			this.euObject = new Atom(v);
		}

		public ValueE(XnObject euObject) {
			this.euObject = euObject;
		}

		public Expression Closure(ClosureManager manager) {
			return this;
		}

		public XnObject Eval(Scope scope) {
			return this.euObject;
		}

		public IEnumerable<XnObject> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}