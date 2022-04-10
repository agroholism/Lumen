using Lumen.Lang;
using Lumen.Lang.Expressions;
using System.Collections.Generic;

namespace Lumen.Lmi {
	internal class PrivateDeclaration : Expression {
		private readonly Expression expression;

		public PrivateDeclaration(Expression expression) {
			this.expression = expression;
		}

		public Expression Closure(ClosureManager manager) {
			return new PrivateDeclaration(this.expression.Closure(manager));
		}
		
		public IValue Eval(Scope scope) {
			ClosureManager manager = new ClosureManager(scope);
			this.expression.Closure(manager);
			foreach (var declaration in manager.Declarations) {
				scope.DeclarePrivate(declaration);
			}

			return this.expression.Eval(scope);
		}

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			return this.expression.EvalWithYield(scope);
		}
	}
}