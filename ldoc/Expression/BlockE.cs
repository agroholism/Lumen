using System;
using System.Linq;
using System.Collections.Generic;

namespace ldoc {
    public class BlockE : Expression {
        public List<Expression> expressions;
        internal Boolean saved = true;

        public BlockE() {
            this.expressions = new List<Expression>();
        }

        public BlockE(List<Expression> Expressions) {
            this.expressions = Expressions;
        }

        public void Add(Expression Expression) {
            this.expressions.Add(Expression);
        }

        public override String ToString() {

            return "{ " + String.Join(Environment.NewLine, this.expressions.Select(i => "\t" + i.ToString())) + " }";
        }
    }
}
