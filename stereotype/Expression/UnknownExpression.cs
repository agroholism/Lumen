using Lumen.Lang.Expressions;
using Lumen.Lang.Std;
using System;
using System.Collections.Generic;

namespace Stereotype {
	class UnknownExpression : Expression {
		public Value Eval(Scope e) {
			return Const.NULL;
		}
		public Expression Optimize(Scope scope) {
			return this;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			return this;
		}

		public override String ToString() {
			return "null";
		}
	}
}
