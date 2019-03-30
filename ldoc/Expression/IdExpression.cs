using System;
using System.Collections.Generic;

namespace ldoc {
    public class IdExpression : Expression {
        public String id;
        public Int32 line;
        public String file;

        public IdExpression(String id, Int32 line, String file) {
            this.id = id;
            this.line = line;
            this.file = file;
        }
     
        public override String ToString() {
            return this.id;
        }
    }
}
