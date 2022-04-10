using System;
using System.Collections.Generic;

namespace Lumen.Lang.Expressions {
	public sealed class Return : Exception, Expression {
		private readonly Expression expression;

		public IValue Result { get; private set; }

		public Return(Expression expression) {
			this.expression = expression;
		}

		public Return(IValue result) {
			this.Result = result;
		}

		public Return(IValue result, Expression expression) {
			this.Result = result;
			this.expression = expression;
		}

		public IValue Eval(Scope e) {
			this.Result = this.expression.Eval(e);
			throw this;
		}

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			IEnumerable<IValue> expressionEvaluationResults = this.expression.EvalWithYield(scope);
			
			foreach(IValue evaluationResult in expressionEvaluationResults) {
				if (evaluationResult is GeneratorExpressionTerminalResult terminalResult) {
					this.Result = terminalResult.Value;
					break;
				}

				yield return evaluationResult;
			}

			throw this;
		}

		public Expression Closure(ClosureManager manager) {
			return new Return(this.expression?.Closure(manager)) {
				Result = this.Result
			};
		}

		public override String ToString() {
			return "return " + (this.expression?.ToString() ?? this.Result?.ToString());
		}
	}
}
