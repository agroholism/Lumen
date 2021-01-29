using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class Yield : Expression {
		private Expression expression;

		public Yield(Expression expression) {
			this.expression = expression;
		}

		public Expression Closure(ClosureManager manager) {
			manager.HasYield = true;
			return new Yield(this.expression.Closure(manager));
		}

		public Value Eval(Scope e) {
			return this.expression.Eval(e);
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			yield return this.expression.Eval(scope);

			if (scope.IsExsists(Constants.YIELD_EXCEPTION_SPECIAL_NAME)) {
				Value ex = scope[Constants.YIELD_EXCEPTION_SPECIAL_NAME];
				scope.Remove(Constants.YIELD_EXCEPTION_SPECIAL_NAME);
				throw ex.ToException();
			}
			else if (scope.IsExsists(Constants.YIELD_VALUE_SPECIAL_NAME)) {
				yield return new GeneratorExpressionTerminalResult(scope[Constants.YIELD_VALUE_SPECIAL_NAME]);
				scope.Remove(Constants.YIELD_VALUE_SPECIAL_NAME);
			}
			else {
				yield return new GeneratorExpressionTerminalResult(Const.UNIT);
			}
		}
	}
}