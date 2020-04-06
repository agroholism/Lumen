using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Lmi {
    internal class MapLiteral : Expression {
        private Dictionary<Expression, Expression> exps;

        public MapLiteral(Dictionary<Expression, Expression> exps) {
            this.exps = exps;
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

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}