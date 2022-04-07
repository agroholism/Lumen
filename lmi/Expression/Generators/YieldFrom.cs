using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class YieldFrom : Expression {
		private Expression expression;

		public YieldFrom(Expression expression) {
			this.expression = expression;
		}

		public Expression Closure(ClosureManager manager) {
			manager.HasYield = true;
			return new YieldFrom(this.expression.Closure(manager));
		}

		public Value Eval(Scope e) {
			return this.expression.Eval(e);
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			IEnumerable<Value> flow = this.expression.Eval(scope).ToFlow(scope);
			
			if (flow is CustomFlow) {
				FlowAutomat automat = flow.GetEnumerator() as FlowAutomat;

				while (automat.MoveNext()) {
					yield return automat.Current;
				}

				Value result = automat.Result;
				if (result != null) {
					yield return new GeneratorExpressionTerminalResult(result);
				}
			} else {
				foreach (Value i in this.expression.Eval(scope).ToFlow(scope)) {
					yield return i;
				}
			}
		}
	}
}