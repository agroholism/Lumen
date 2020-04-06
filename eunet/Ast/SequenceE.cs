using System.Collections.Generic;
using System.Linq;
using Argent.Xenon.Runtime;

namespace Argent.Xenon.Ast {
	internal class SequenceE : Expression {
		private List<Expression> elements;

		public SequenceE(List<Expression> elements) {
			this.elements = elements;
		}

		public Expression Closure(ClosureManager manager) {
			return new SequenceE(this.elements.Select(x => x.Closure(manager)).ToList());
		}

		public XnObject Eval(Scope scope) {
			return new XnList(this.elements.Select(i => i.Eval(scope)).ToList());
		}

		public IEnumerable<XnObject> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}