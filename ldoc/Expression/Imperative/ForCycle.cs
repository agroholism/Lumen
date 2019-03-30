using System.Collections.Generic;
using System;
using System.Linq;

namespace ldoc {
    internal class ForCycle : Expression {
        public Expression expression;
        public Expression body;
        private readonly String varName;
        private readonly Expression varType;
        private readonly Boolean declaredVar;

        public ForCycle(String varName, Expression varType, Boolean declaredVar, Expression expressions, Expression statement) {
            this.varName = varName;
            this.varType = varType;
            this.declaredVar = declaredVar;
            this.expression = expressions;
            this.body = statement;
        }
    }
}