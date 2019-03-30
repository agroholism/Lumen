using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Light {
    internal class ConditionE : Expression {
        public Expression condition;
        public Expression falseExpression;
        public Expression trueExpression;

        public ConditionE(Expression condition, Expression trueExpression, Expression falseExpression) {
            this.condition = condition;
            this.trueExpression = trueExpression;
            this.falseExpression = falseExpression;
        }

        public Value Eval(Scope scope) {
            Boolean result = this.condition.Eval(scope).ToBoolean();

            return result ? this.trueExpression.Eval(scope) : this.falseExpression.Eval(scope);
        }

        public Expression Closure(List<String> visible, Scope thread) {
            return new ConditionE(this.condition.Closure(visible, thread), this.trueExpression.Closure(visible, thread), this.falseExpression.Closure(visible, thread));
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