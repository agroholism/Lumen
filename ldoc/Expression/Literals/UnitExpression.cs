using System;
using System.Collections.Generic;

namespace ldoc {
    public class UnitExpression : Expression {
        public static UnitExpression Instance { get; } = new UnitExpression();

        private UnitExpression() {

        }

        public override String ToString() {
            return "()";
        }
    }
}