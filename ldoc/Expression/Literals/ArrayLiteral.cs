using System.Collections.Generic;
using System.Linq;
using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace ldoc {
    internal class ArrayE : Expression {
        private List<Expression> elements;

        public ArrayE(List<Expression> elements) {
            this.elements = elements;
        }
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public Expression Closure(ClosureManager manager) {
            return new ArrayE(this.elements.Select(i => i.Closure(manager)).ToList());
        }

        public Value Eval(Scope e) {
            return new Array(this.elements.Select(i => i.Eval(e)).ToList());
        }
    }
}