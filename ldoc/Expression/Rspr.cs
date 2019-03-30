using System;

namespace ldoc {
    internal class SpreadE : Expression {
        internal Expression expression;
        public Int32 i;
  
        public SpreadE(Expression expression) {
            this.expression = expression;
            if (expression is SpreadE) {
                this.i += ((SpreadE)expression).i + 1;
            }
        }
    }
}