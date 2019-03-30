using System;
using System.Collections.Generic;

namespace ldoc {
    public class BinaryExpression : Expression {
        public Expression expressionOne;
        public Expression expressionTwo;
        public String operation;
        public Int32 line;
        public String fileName;

        public BinaryExpression(Expression expressionOne, Expression expressionTwo, String operation, Int32 line, String file) {
            this.expressionOne = expressionOne;
            this.expressionTwo = expressionTwo;
            this.operation = operation;
            this.line = line;
            this.fileName = file;
        }
    }
}