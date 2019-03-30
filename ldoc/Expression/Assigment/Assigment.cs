using System;
using System.Collections.Generic;

namespace ldoc {
    /// <summary> Assigment operator. </summary>
    public class Assigment : Expression {
        internal String variableName;
        internal Expression assignableExpression;
        internal Int32 line;
        internal String file;

        public Assigment(String variableName, Expression assignableExpression, Int32 line, String file) {
            this.variableName = variableName;
            this.assignableExpression = assignableExpression;
            this.line = line;
            this.file = file;
        }
    }
}