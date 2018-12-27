using System;
using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	[Serializable]
	internal class ExpressionE : Expression {
		internal Expression expression;

		public ExpressionE(Expression expression) {
			this.expression = expression;
		}

		public Expression Closure(List<System.String> visible, Scope scope) {
			throw new System.NotImplementedException();
		}
		public Expression Optimize(Scope scope) {
			return this;
		}
		public Value Eval(Scope e) {
			throw new System.NotImplementedException();
		}
	}
}