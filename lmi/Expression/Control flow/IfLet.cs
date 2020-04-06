using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class IfLet : Expression {
		private IPattern pattern;
		private Expression assinableExpression;
		private Expression trueExpression;
		private Expression falseExpression;

		public IfLet(IPattern pattern, Expression assinableExpression, Expression trueBody, Expression falseBody) {
			this.pattern = pattern;
			this.assinableExpression = assinableExpression;
			this.trueExpression = trueBody;
			this.falseExpression = falseBody;
		}

		public Expression Closure(ClosureManager manager) {
			return new IfLet(
				this.pattern.Closure(manager) as IPattern,
				this.assinableExpression.Closure(manager),
				this.trueExpression.Closure(manager),
				this.falseExpression.Closure(manager)
				);
		}

		public Value Eval(Scope scope) {
			Value matchable = this.assinableExpression.Eval(scope);
			Boolean result = this.pattern.Match(matchable, scope).Success;

			return result ? this.trueExpression.Eval(scope) : this.falseExpression.Eval(scope);
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}