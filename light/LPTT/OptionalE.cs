using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	internal class OptionalE : Expression {
		private Expression result;

		public OptionalE(Expression result) {
			this.result = result;
		}

		public Expression Closure(List<System.String> visible, Scope scope) {
			return new OptionalE(this.result.Closure(visible, scope));
		}

		public Value Eval(Scope e) {
			Optional result = Optional.Create(this.result.Eval(e));
			return result;
		}

		public Expression Optimize(Scope scope) {
			throw new System.NotImplementedException();
		}
	}
}