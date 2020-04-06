using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace ldoc {
    internal class MapE : Expression {
        private Dictionary<Expression, Expression> exps;

        public MapE(Dictionary<Expression, Expression> exps) {
            this.exps = exps;
        }
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public Expression Closure(ClosureManager manager) {
            return this;
        }

        public Value Eval(Scope e) {
            Dictionary<Value, Value> res = new Dictionary<Value, Value>();

            foreach(KeyValuePair<Expression, Expression> i in this.exps) {
                res.Add(i.Key.Eval(e), i.Value.Eval(e));
            }

            return new Map(res);
        }
    }
}