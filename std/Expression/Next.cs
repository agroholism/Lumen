using System;
using System.Collections.Generic;

namespace Lumen.Lang.Expressions {
	public class Next : Exception, Expression {
		public static Next Instance { get; } = new Next();

		private Next() {

		}

        public Value Eval(Scope e) {
            throw this;
        }

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public Expression Closure(ClosureManager manager) {
            return this;
        }
    }
}