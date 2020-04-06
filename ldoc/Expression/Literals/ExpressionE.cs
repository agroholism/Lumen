using System;
using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;
using String = System.String;

namespace ldoc {
    internal class ExpressionE : Expression {
        internal Expression expression;

        public ExpressionE(Expression expression) {
            this.expression = expression;
        }
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public Expression Closure(ClosureManager manager) {
            throw new NotImplementedException();
        }
        public Expression Optimize(Scope scope) {
            return this;
        }
        public Value Eval(Scope e) {
            throw new NotImplementedException();
        }
    }
}