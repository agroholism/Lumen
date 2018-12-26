using System;
using System.Collections.Generic;
using Lumen;
using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	[Serializable]
	internal class Once : Expression {
		private Expression expression;
		private Boolean isDepricated;

		public Once(Expression expression) {
			this.expression = expression;
		}

		public Expression Closure(List<String> visible, Scope scope) {
			return new Once(this.expression.Closure(visible, scope));
		}
		public Expression Optimize(Scope scope) {
			return this;
		}
		public Value Eval(Scope e) { 
			if(this.isDepricated) {
				return Const.NULL;
			}
			this.isDepricated = true;
			return this.expression.Eval(e);
		}
	}
}