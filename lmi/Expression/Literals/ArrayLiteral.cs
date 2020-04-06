using System.Collections.Generic;
using System.Linq;
using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Lmi {
    internal class ArrayLiteral : Expression {
        private List<Expression> elements;

        public ArrayLiteral(List<Expression> elements) {
            this.elements = elements;
        }

        public Expression Closure(ClosureManager manager) {
            return new ArrayLiteral(this.elements.Select(i => i.Closure(manager)).ToList());
        }

        public Value Eval(Scope e) {
			IEnumerable<Value> ToStream(List<Expression> exps) {
				foreach (Expression exp in exps) {
					if (exp is From from) {
						foreach (Value i in from.Eval(e).ToStream(e)) {
							yield return i;
						}
					}
					else {
						yield return exp.Eval(e);
					}
				}
			}

			return new Array(ToStream(this.elements).ToList());
        }

		public override System.String ToString() {
			return $"[|{System.String.Join(", ", this.elements)}|]";
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			List<Value> result = new List<Value>();

			foreach(var i in this.elements) {
				foreach(var j in i.EvalWithYield(scope)) {
					if(j is CurrGeenVal cgv) {
						result.Add(cgv.Value);
					} else {
						yield return j;
					}
				}
			}

			yield return new CurrGeenVal(new Array(result));
		}
	}
}