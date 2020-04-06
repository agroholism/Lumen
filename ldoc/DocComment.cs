using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace ldoc {
    internal class DocComment : Expression {
        public Expression Inner;
        public string doc;

        public DocComment(Expression inner, string doc) {
            this.Inner = inner;
            this.doc = doc;
        }

		public Expression Closure(ClosureManager manager) {
			return this;
		}

		public Value Eval(Scope e) {
			throw new System.NotImplementedException();
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}