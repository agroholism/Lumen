using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace ldoc {
    internal class ConditionE : Expression {
        public Expression condition;
        public Expression falseExpression;
        public Expression trueExpression;
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public ConditionE(Expression condition, Expression trueExpression, Expression falseExpression) {
            this.condition = condition;
            this.trueExpression = trueExpression;
            this.falseExpression = falseExpression;
        }

        public Value Eval(Scope scope) {
            Boolean result = this.condition.Eval(scope).ToBoolean();

            return result ? this.trueExpression.Eval(scope) : this.falseExpression.Eval(scope);
        }

        public Expression Closure(ClosureManager manager) {
            return new ConditionE(this.condition.Closure(manager), this.trueExpression.Closure(manager), this.falseExpression.Closure(manager));
        }

        public override String ToString() {
            String result = "if " + this.condition.ToString() + ": " + Environment.NewLine + "\t" + this.trueExpression.ToString();

            if (this.falseExpression != null && !(this.falseExpression is UnitExpression)) {
                result += Environment.NewLine + "else " + Environment.NewLine + this.falseExpression.ToString();
            }

            return result;
        }
    }
}