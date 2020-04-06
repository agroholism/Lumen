using System.Collections.Generic;
using Argent.Xenon.Runtime;

namespace Argent.Xenon.Ast {
	internal class GetIndexE : Expression {
		internal Expression sliced;
		internal List<Expression> indices;
		private System.Int32 line;
		private System.String fileName;

		public GetIndexE(Expression sliced, List<Expression> indices, System.Int32 line, System.String fileName) {
			this.sliced = sliced;
			this.indices = indices;
			this.line = line;
			this.fileName = fileName;
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