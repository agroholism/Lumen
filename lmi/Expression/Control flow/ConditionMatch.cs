using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class ConditionMatch : Expression {
		private IPattern pattern;
		private Expression assinableExpression;
		private Expression trueExpression;
		private Expression falseExpression;

		public ConditionMatch(IPattern pattern, Expression assinableExpression, Expression trueBody, Expression falseBody) {
			this.pattern = pattern;
			this.assinableExpression = assinableExpression;
			this.trueExpression = trueBody;
			this.falseExpression = falseBody;
		}

		public Expression Closure(ClosureManager manager) {
			return new ConditionMatch(
				this.pattern.Closure(manager) as IPattern,
				this.assinableExpression.Closure(manager),
				this.trueExpression.Closure(manager),
				this.falseExpression.Closure(manager)
				);
		}

		public Value Eval(Scope scope) {
			Value matchable = this.assinableExpression.Eval(scope);
			Boolean result = this.pattern.Match(matchable, scope).IsSuccess;

			return result ? this.trueExpression.Eval(scope) : this.falseExpression.Eval(scope);
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			IEnumerable<Value> conditionEvaluationResult = this.assinableExpression.EvalWithYield(scope);

			Value valueToCheck = Const.UNIT;
			foreach (Value result in conditionEvaluationResult) {
				if (result is GeneratorExpressionTerminalResult generatorResult) {
					valueToCheck = generatorResult.Value;
					break;
				}

				yield return result;
			}

			Boolean matchResult = this.pattern.Match(valueToCheck, scope).IsSuccess;

			IEnumerable<Value> expressionResults = matchResult ? this.trueExpression.EvalWithYield(scope) : this.falseExpression.EvalWithYield(scope);

			foreach (Value result in expressionResults) {
				yield return result;
			}
		}
	}
}