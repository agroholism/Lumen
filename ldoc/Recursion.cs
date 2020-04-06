using System.Collections.Generic;
using System.Linq;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace ldoc {
	internal class Recursion : Expression {
		private List<Expression> args;
		private System.String fileName;
		private System.Int32 line;

		public Recursion(List<Expression> args, System.String fileName, System.Int32 line) {
			this.args = args;
			this.fileName = fileName;
			this.line = line;
		}
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public Expression Closure(ClosureManager manager) {
			return new Recursion(this.args.Select(i => i.Closure(manager)).ToList(), this.fileName, this.line);
		}

		public Value Eval(Scope e) {
			throw new RecursionException(this.args.Select(i => i.Eval(e)).ToArray());
		}
	}
}