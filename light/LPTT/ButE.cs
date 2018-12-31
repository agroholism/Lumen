using System;
using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	[Serializable]
	internal class ButE : Expression {
		internal Expression res;
		internal Expression expression;

		public Expression Optimize(Scope scope) {
			return this;
		}

		public ButE(Expression res, Expression expression) {
			this.res = res;
			this.expression = expression;
		}

		public Expression Closure(List<System.String> visible, Scope scope) {
			return new ButE(this.res.Closure(visible, scope), this.expression.Closure(visible, scope));
		}

		public Value Eval(Scope e) {
			this.res.Eval(e);
			return this.expression.Eval(e);
		}

		public override System.String ToString() {
			return this.expression.ToString();
		}
	}
}