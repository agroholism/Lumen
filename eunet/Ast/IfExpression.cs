using System.Collections.Generic;
using Argent.Xenon.Runtime;

namespace Argent.Xenon.Ast {
	internal class IfExpression : Expression {
		private Dictionary<Expression, Expression> conditionalBlocks;
		private Expression falseBody;

		public IfExpression(Dictionary<Expression, Expression> conditionalBlocks, Expression falseBody) {
			this.conditionalBlocks = conditionalBlocks;
			this.falseBody = falseBody;
		}

		public Expression Closure(ClosureManager manager) {
			Dictionary<Expression, Expression> exps = new Dictionary<Expression, Expression>();
			foreach (KeyValuePair<Expression, Expression> item in this.conditionalBlocks) {
				exps[item.Key.Closure(manager)] = item.Value.Closure(manager);
			}
			return new IfExpression(exps, this.falseBody.Closure(manager));
		}

		public XnObject Eval(Scope scope) {
			foreach(KeyValuePair<Expression, Expression> i in this.conditionalBlocks) {
				if(XnStd.AsBool(i.Key.Eval(scope))) {
					return i.Value.Eval(scope);
				}
			}

			return this.falseBody.Eval(scope);
		}

		public IEnumerable<XnObject> EvalWithYield(Scope scope) {
			foreach (KeyValuePair<Expression, Expression> i in this.conditionalBlocks) {
				if (XnStd.AsBool(i.Key.Eval(scope))) {
					var x = i.Value.EvalWithYield(scope);
					foreach(var it in x) {
						yield return it;
					}
					yield break;
				}
			}

			var z = this.falseBody.EvalWithYield(scope);
			foreach (var it in z) {
				yield return it;
			}
		}
	}
}