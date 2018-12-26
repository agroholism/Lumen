using System;
using System.Collections.Generic;

using StandartLibrary;
using StandartLibrary.Expressions;

namespace Stereotype {
	public class Nop : Expression {
		public static Nop Instance { get; } = new Nop();

		private Nop() {

		}

		public Expression Closure(List<String> visible, Scope scope) {
			return this;
		}

		public Value Eval(Scope e) {
			return Const.NULL;
		}

		public Expression Optimize(Scope scope) {
			return this;
		}

		public override String ToString() {
			return "";
		}
	}
}