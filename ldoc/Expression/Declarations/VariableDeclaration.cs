using System;
using System.Collections.Generic;

namespace ldoc {
    public class VariableDeclaration : Expression {
        public String variableName;
        public Expression variableType;
        public Expression assignableExpression;
        public String file;
        public Int32 line;

        public VariableDeclaration(String variableName, Expression variableType, Expression assignableExpression, String file, Int32 line) {
            this.variableName = variableName;
            this.variableType = variableType;
            this.assignableExpression = assignableExpression;
            this.line = line;
            this.file = file;
        }
    }
}