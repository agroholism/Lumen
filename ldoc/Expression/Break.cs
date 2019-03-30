using System;
using System.Collections.Generic;

namespace ldoc {
    public class Break : Exception, Expression {
        private Int32 label;

        public Break(Int32 label) : base("unexpected break with label") {
            this.label = label;
        }

        public Int32 UseLabel() {
            this.label--;
            return this.label;
        }
    }
}