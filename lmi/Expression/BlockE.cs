using System;
using System.Linq;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Lmi {
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
			return String.Join(Environment.NewLine, this.expressions);

        }

        public Expression Closure(ClosureManager manager) {
            return new BlockE(this.expressions.Select(expression => expression.Closure(manager)).ToList());
        }

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			for (Int32 i = 0; i < this.expressions.Count - 1; i++) {
				IEnumerable<Value> x = this.expressions[i].EvalWithYield(scope);
				foreach (Value it in x) {
					if(it is CurrGeenVal) {
						continue;
					}
					yield return it;
				}
			}

			IEnumerable<Value> z = this.expressions[this.expressions.Count - 1].EvalWithYield(scope);
			foreach (Value it in z) {
				yield return it;
			}
		}
	}
}
