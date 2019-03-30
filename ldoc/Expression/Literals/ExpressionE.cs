using System;
using System.Collections.Generic;

namespace ldoc {
    internal class ExpressionE : Expression {
        internal Expression expression;

        public ExpressionE(Expression expression) {
            this.expression = expression;
        }
    }
}