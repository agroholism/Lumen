using System.Collections.Generic;

namespace ldoc {
    internal class AddE : Expression {
        private Expression result;
        private Expression right;

        public AddE(Expression result, Expression right) {
            this.result = result;
            this.right = right;
        }
    }
}