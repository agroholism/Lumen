using System;
using System.Collections.Generic;

namespace ldoc {
    internal class WhileExpression : Expression {
        internal Expression condition;
        internal Expression body;

        internal WhileExpression(Expression condition, Expression body) {
            this.condition = condition;
            this.body = body;
        }
    }
}