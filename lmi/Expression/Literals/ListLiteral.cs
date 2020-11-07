using System.Collections.Generic;
using System.Linq;
using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Lmi {
    internal class ListE : Expression {
        private List<Expression> elements;

        public ListE(List<Expression> elements) {
            this.elements = elements;
        }

        public Expression Closure(ClosureManager manager) {
            return new ListE(this.elements.Select( i=> i.Closure(manager)).ToList());
        }

        public Value Eval(Scope e) {
			IEnumerable<Value> ToStream(List<Expression> exps) {
				foreach(Expression exp in exps) {
					if(exp is From from) {
						foreach(Value i in from.Eval(e).ToStream(e)) {
							yield return i;
						}
					} else {
						yield return exp.Eval(e);
					}
				}
			}

            return new List(ToStream(this.elements));
        }

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			List<Value> result = new List<Value>();

			foreach (Expression i in this.elements) {
				foreach (Value j in i.EvalWithYield(scope)) {
					if (j is GeneratorExpressionTerminalResult cgv) {
						result.Add(cgv.Value);
					}
					else {
						yield return j;
					}
				}
			}

			yield return new GeneratorExpressionTerminalResult(new List(result));
		}

		public override System.String ToString() {
			return $"[{System.String.Join(", ", this.elements)}]";
		}
	}
}