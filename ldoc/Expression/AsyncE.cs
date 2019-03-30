using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ldoc {
    internal class AsyncE : Expression {
        private Expression expression;

        public AsyncE(Expression expression) {
            this.expression = expression;
        }
    }
}