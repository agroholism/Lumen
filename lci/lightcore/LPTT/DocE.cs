using System.Collections.Generic;
using StandartLibrary;
using StandartLibrary.Expressions;

namespace Stereotype {
	public class DocE : Expression {
		public System.String v;
		public Expression expression;

		public DocE(System.String v, Expression expression) {
			this.v = v;
			this.expression = expression;
		}

		public Expression Closure(List<System.String> visible, Scope scope) {
			return this.expression.Closure(visible, scope);
		}

		public Value Eval(Scope e) {
			return this.expression.Eval(e);
		}

		public Expression Optimize(Scope scope) {
			return this.expression.Optimize(scope);
		}
	}
}