using System;
using System.Collections.Generic;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class Condition : Expression {
        public Expression conditionExpression;
        public Expression falseExpression;
        public Expression trueExpression;

        public Condition(Expression condition, Expression trueExpression, Expression falseExpression) {
            this.conditionExpression = condition;
            this.trueExpression = trueExpression;
            this.falseExpression = falseExpression;
        }

        public Value Eval(Scope scope) {
            Boolean result = this.conditionExpression.Eval(scope).ToBoolean();

            return result ? this.trueExpression.Eval(scope) : this.falseExpression.Eval(scope);
        }

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			IEnumerable<Value> conditionEvaluationResult = this.conditionExpression.EvalWithYield(scope);

			Value condition = Const.UNIT;
			foreach (Value result in conditionEvaluationResult) {
				if (result is CurrGeenVal cgv) {
					condition = cgv.Value;
					break;
				}

				yield return result;
			}

			IEnumerable<Value> expressionResults = condition.ToBoolean() ? this.trueExpression.EvalWithYield(scope) : this.falseExpression.EvalWithYield(scope);

			foreach (Value result in expressionResults) {
				yield return result;
			}
		}

		public Expression Closure(ClosureManager manager) {
			// Here problem
            return new Condition(this.conditionExpression.Closure(manager), this.trueExpression.Closure(manager), this.falseExpression.Closure(manager));
        }

        public override String ToString() {
            String result = "if " + this.conditionExpression.ToString() + ": " + Environment.NewLine + "\t" + this.trueExpression.ToString();

            if (this.falseExpression != null && !(this.falseExpression is UnitLiteral)) {
                result += Environment.NewLine + "else: " + Environment.NewLine + "\t" + this.falseExpression.ToString();
            }

            return result;
        }
	}
}