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

		public IValue Eval(Scope e) {
			return this.expression.Eval(e);
		}

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			IEnumerable<IValue> flow = this.expression.Eval(scope).ToFlow(scope);
			
			if (flow is CustomFlow) {
				FlowAutomat automat = flow.GetEnumerator() as FlowAutomat;

				while (automat.MoveNext()) {
					yield return automat.Current;
				}

				IValue result = automat.Result;
				if (result != null) {
					yield return new GeneratorExpressionTerminalResult(result);
				}
			} else {
				foreach (IValue i in this.expression.Eval(scope).ToFlow(scope)) {
					yield return i;
				}
			}
		}
	}
}