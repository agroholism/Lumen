using System;
using System.Collections.Generic;

namespace ldoc {
    public sealed class Return : Exception, Expression {
        private readonly Expression expression;

        public Return(Expression expression) {
            this.expression = expression;
        }
    }
}
