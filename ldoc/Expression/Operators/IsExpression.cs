using System;
using System.Collections.Generic;

namespace ldoc {
    class IsExpression : Expression {
        internal Expression Expression;
        internal Expression type;

        public IsExpression(Expression Expression, Expression type) {
            this.Expression = Expression;
            this.type = type;
        }

        public override String ToString() {
            return this.Expression.ToString() + " is " + this.type;
        }
    }
}
