using System;
using System.Collections.Generic;

namespace ldoc {
    internal class DotExpression : Expression {
        internal Expression expression;
        internal String nameVariable;
        private readonly String fileName;
        private readonly Int32 line;

        public DotExpression(Expression expression, String nameVariable, String fileName, Int32 line) {
            this.expression = expression;
            this.nameVariable = nameVariable;
            this.fileName = fileName;
            this.line = line;
        }

        public override String ToString() {
            return this.expression.ToString() + "." + this.nameVariable;
        }
    }
}