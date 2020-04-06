using System.Collections.Generic;
using Argent.Xenon.Runtime;

namespace Argent.Xenon.Ast {
	internal class ForExpression : Expression {
		private System.String nameVariable;
		private Expression containerExpression;
		private Expression expression;

		public ForExpression(System.String nameVariable, Expression containerExpression, Expression expression) {
			this.nameVariable = nameVariable;
			this.containerExpression = containerExpression;
			this.expression = expression;
		}

		public Expression Closure(ClosureManager manager) {
			ClosureManager man2 = manager.Clone();
			man2.Declare(this.nameVariable);

			var res = new ForExpression(this.nameVariable, this.containerExpression.Closure(manager), this.expression.Closure(man2));

			if(manager.HasYield == false && man2.HasYield) {
				manager.HasYield = true;
			}

			return res;
		}

		public XnObject Eval(Scope scope) {
			XnObject cont = this.containerExpression.Eval(scope);

			scope.DeclareVariable(this.nameVariable, XnStd.ObjectType);
			foreach (XnObject i in XnStd.AsSequence(cont)) {
				scope.Assign(this.nameVariable, i);
				this.expression.Eval(scope);
			}

			return null;
		}

		public IEnumerable<XnObject> EvalWithYield(Scope scope) {
			XnObject cont = this.containerExpression.Eval(scope);

			scope.DeclareVariable(this.nameVariable, XnStd.ObjectType);
			foreach (XnObject i in XnStd.AsSequence(cont)) {
				scope.Assign(this.nameVariable, i);
				var z = this.expression.EvalWithYield(scope);
				foreach(var it in z) {
					yield return it;
				}
			}
		}
	}
}