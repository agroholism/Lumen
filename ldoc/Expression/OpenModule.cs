using System.Collections.Generic;

namespace ldoc {
    internal class OpenModule : Expression {
        private readonly Expression expression;

        public OpenModule(Expression expression) {
            this.expression = expression;
        }
    }
}