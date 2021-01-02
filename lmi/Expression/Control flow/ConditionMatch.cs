using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;
using Lumen.Lang.Patterns;

namespace Lumen.Lmi {
	internal class ConditionMatch : Expression {
		private readonly IPattern pattern;
		private readonly Expression assinableExpression;
		private readonly Expression trueExpression;
		private readonly Expression falseExpression;

		public ConditionMatch(IPattern pattern, Expression assinableExpression, Expression trueBody, Expression falseBody) {
			this.pattern = pattern;
			this.assinableExpression = assinableExpression;
			this.trueExpression = trueBody;
			this.falseExpression = falseBody;
		}

		public Expression Closure(ClosureManager manager) {
			// Right expression - first
			Expression closuredAssignableExpression = this.assinableExpression?.Closure(manager);
			IPattern closuredPattern = this.pattern.Closure(manager) as IPattern;

			// Because trueExpression and falseExpression can influence on each other without isolated managers
			ClosureManager trueExpressionManager = manager.Clone();
			Expression closuredTrueExpression = this.trueExpression.Closure(trueExpressionManager);

			// But there are we use old manager so we can aviod additional manager
			Expression closuredFalseExpression = this.falseExpression.Closure(manager);

			ConditionMatch result = new ConditionMatch(closuredPattern, closuredAssignableExpression, closuredTrueExpression, closuredFalseExpression);
			// And just move results in base manager
			manager.Assimilate(trueExpressionManager);
			return result;
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