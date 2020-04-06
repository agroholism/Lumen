using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace ldoc {
    internal class ListGenerator : Expression {
        private SequenceGenerator sequenceGenerator;

        public ListGenerator(SequenceGenerator sequenceGenerator) {
            this.sequenceGenerator = sequenceGenerator;
        }
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public Expression Closure(ClosureManager manager) {
            return new ListGenerator(this.sequenceGenerator.Closure(manager) as SequenceGenerator);
        }

        public Value Eval(Scope e) {
			var list = new List(this.sequenceGenerator.Generator(e));
			return list;
        }
    }
}