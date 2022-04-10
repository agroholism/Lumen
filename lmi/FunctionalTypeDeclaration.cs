using Lumen.Lang;
using Lumen.Lang.Expressions;
using System.Collections.Generic;

namespace Lumen.Lmi {
	internal class FunctionalTypeDeclaration : Expression {
		private readonly string name;
		private readonly Expression expression;

		public FunctionalTypeDeclaration(string name, Expression expression) {
			this.name = name;
			this.expression = expression;

		}

		public Expression Closure(ClosureManager manager) {
			manager.Declare(this.name);
			return this.expression.Closure(manager);
		}

		public IValue Eval(Scope scope) {
			Fun function = this.expression.Eval(scope).ToFunction(scope);

			IValue type = new FunctionalType(this.name, function);

			scope.Bind(this.name, type);

			return type;
		}

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			throw new System.NotImplementedException();
		}
	}
}