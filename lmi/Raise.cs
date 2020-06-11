using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class Raise : Expression {
		private Expression expression;

		public Raise(Expression expression) {
			this.expression = expression;
		}

		public Expression Closure(ClosureManager manager) {
			return new Raise(this.expression.Closure(manager));
		}

		public Value Eval(Scope scope) {
			Value val = this.expression.Eval(scope);

			System.String message = val.CallMethod("message", scope).ToString();

			LumenException exception = new LumenException(message) {
				AttachedObject = val
			};

			throw exception;
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			yield return this.Eval(scope);
		}
	}
}