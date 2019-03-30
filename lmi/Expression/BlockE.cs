using System;
using System.Linq;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Light {
    public class BlockE : Expression {
        public List<Expression> expressions;

        public BlockE() {
            this.expressions = new List<Expression>();
        }

        public BlockE(List<Expression> Expressions) {
            this.expressions = Expressions;
        }

        public void Add(Expression Expression) {
            this.expressions.Add(Expression);
        }

        public Value Eval(Scope e) {
            for (Int32 i = 0; i < this.expressions.Count - 1; i++) {
                this.expressions[i].Eval(e);
            }

            if (this.expressions.Count > 0) {
                return this.expressions[this.expressions.Count - 1].Eval(e);
            }

            return Const.UNIT;
        }

        public override String ToString() {
            return String.Join(Environment.NewLine, this.expressions.Select(i => "\t" + i.ToString()));
        }

        public Expression Closure(List<String> visible, Scope thread) {
            return new BlockE(this.expressions.Select(expression => expression.Closure(visible, thread)).ToList());
        }
    }
}
