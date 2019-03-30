using System;
using System.Collections.Generic;
using System.Linq;

namespace ldoc {
    internal class Applicate : Expression {
        public Expression callable;
        public List<Expression> argumentsExpression;
        public Int32 line;
        public String fileName;

        public Applicate(Expression callable, List<Expression> argumentsExpression, Int32 line, String fileName) {
            this.callable = callable;
            this.argumentsExpression = argumentsExpression;
            this.line = line;
            this.fileName = fileName;
        }
    }
}