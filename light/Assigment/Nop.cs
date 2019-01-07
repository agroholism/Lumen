using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	public class Nop : Expression {
		public static Nop Instance { get; } = new Nop();

		private Nop() {

		}

		public Expression Closure(List<String> visible, Scope scope) {
			return this;
		}

		public Value Eval(Scope e) {
			return Const.VOID;
		}

		public Expression Optimize(Scope scope) {
			return this;
		}

		public override String ToString() {
			return "";
		}
	}
}