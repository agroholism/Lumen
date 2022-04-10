using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class Condition : Expression {
        private readonly Expression conditionExpression;
        private readonly Expression trueExpression;
        private readonly Expression falseExpression;

        public Condition(Expression condition, Expression trueExpression, Expression falseExpression) {
            this.conditionExpression = condition;
            this.trueExpression = trueExpression;
            this.falseExpression = falseExpression;
        }

        public IValue Eval(Scope scope) {
            Boolean result = this.conditionExpression.Eval(scope).ToBoolean();

            return result ? this.trueExpression.Eval(scope) : this.falseExpression.Eval(scope);
        }

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			IEnumerable<IValue> conditionEvaluationResult = this.conditionExpression.EvalWithYield(scope);

			IValue condition = Const.UNIT;
			foreach (IValue evaluationResult in conditionEvaluationResult) {
				if (evaluationResult is GeneratorExpressionTerminalResult terminalResult) {
					condition = terminalResult.Value;
					break;
				}

				yield return evaluationResult;
			}

			IEnumerable<IValue> expressionResults = condition.ToBoolean() 
                ? this.trueExpression.EvalWithYield(scope) 
                : this.falseExpression.EvalWithYield(scope);

			foreach (IValue expressionResult in expressionResults) {
				yield return expressionResult;
			}
		}

		public Expression Closure(ClosureManager manager) {
			Expression closuredConditionExpression = this.conditionExpression.Closure(manager);

            // Because trueExpression and falseExpression can influence on each other without isolated managers
            ClosureManager trueExpressionManager = manager.Clone();
            Expression closuredTrueExpression = this.trueExpression.Closure(trueExpressionManager);

            // But there are we use old manager so we can aviod additional manager
            Expression closuredFalseExpression = this.falseExpression.Closure(manager);

			Condition result =  new Condition(closuredConditionExpression, closuredTrueExpression, closuredFalseExpression);
            // And just move results in base manager
            manager.Assimilate(trueExpressionManager);
            return result;
        }

        public override String ToString() {
            String result = $"if {this.conditionExpression}:{Environment.NewLine}\t{this.trueExpression}";

            if (this.falseExpression != null && this.falseExpression is not UnitLiteral) {
                result += $"{Environment.NewLine}else:{Environment.NewLine}\t{this.falseExpression}";
            }

            return result;
        }
	}
}