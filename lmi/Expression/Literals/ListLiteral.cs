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

        public IValue Eval(Scope e) {
			IEnumerable<IValue> ToStream(List<Expression> exps) {
				foreach(Expression exp in exps) {
					if(exp is From from) {
						foreach(IValue i in from.Eval(e).ToFlow(e)) {
							yield return i;
						}
					} else {
						yield return exp.Eval(e);
					}
				}
			}

            return new List(ToStream(this.elements));
        }

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			List<IValue> result = new List<IValue>();

			foreach (Expression i in this.elements) {
				foreach (IValue j in i.EvalWithYield(scope)) {
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