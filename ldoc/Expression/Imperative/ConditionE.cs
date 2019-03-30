using System;
using System.Collections.Generic;

namespace ldoc {
    internal class ConditionE : Expression {
        public Expression condition;
        public Expression falseExpression;
        public Expression trueExpression;

        public ConditionE(Expression condition, Expression trueExpression, Expression falseExpression) {
            this.condition = condition;
            this.trueExpression = trueExpression;
            this.falseExpression = falseExpression;
        }
    }
}