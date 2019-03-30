using System;
using System.Collections.Generic;

namespace ldoc {
    internal class DotAssigment : Expression {
        internal Expression rigth;
        internal DotExpression left;
        internal String file;
        internal Int32 line;

        internal DotAssigment(DotExpression left, Expression rigth, String file, Int32 line) {
            this.left = left;
            this.rigth = rigth;
            this.file = file;
            this.line = line;
        }
    }
}