using System.Collections.Generic;
using System.Linq;
using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace ldoc {
    internal class ListE : Expression {
        private List<Expression> elements;

        public ListE(List<Expression> elements) {
            this.elements = elements;
        }
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public Expression Closure(ClosureManager manager) {
            return this;
        }

        public Value Eval(Scope e) { 
            return new List(LinkedList.Create(this.elements.Select(i => i.Eval(e))));
        }
    }
}