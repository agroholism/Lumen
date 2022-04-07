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

		public Value Eval(Scope scope) {
			Fun function = this.expression.Eval(scope).ToFunction(scope);

			Value type = new FunctionalType(this.name, function);

			scope.Bind(this.name, type);

			return type;
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			throw new System.NotImplementedException();
		}
	}
}