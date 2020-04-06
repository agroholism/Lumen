using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Lmi {
    internal class OpenModule : Expression {
        private readonly Expression expression;

        public OpenModule(Expression expression) {
            this.expression = expression;
        }

        public Expression Closure(ClosureManager manager) {
            return new OpenModule(this.expression.Closure(manager));
        }

        public Value Eval(Scope e) {
            Value value = this.expression.Eval(e);

            if (value is Module module) {
                foreach (KeyValuePair<System.String, Value> i in module.Members) {
                    e[i.Key] = i.Value;
                }
                return Const.UNIT;
            }

            throw new LumenException("it's not a module");
        }

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}