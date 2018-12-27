using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	internal class Auto : Expression {
		public Value Eval(Scope e) {
			return Const.NULL;
		}

		public Expression Closure(List<String> visible, Scope scope) {
			return this;
		}


		public Expression Optimize(Scope scope) {
			return this;
		}
	}
}
