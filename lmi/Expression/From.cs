using Lumen.Lang.Expressions;
using Lumen.Lang;
using System;
using System.Collections.Generic;

namespace Lumen.Lmi {
    internal class From : Expression {
        public Expression Closure(ClosureManager manager) {
            return this;
        }
        internal Expression expression;
        public Int32 i;
  
        public From(Expression expression) {
            this.expression = expression;
            if (expression is From) {
                this.i += ((From)expression).i + 1;
            }
        }

        public Int32 Get() {
            return this.i;
        }

        public IValue Eval(Scope e) {
            return this.expression.Eval(e);
        }

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}