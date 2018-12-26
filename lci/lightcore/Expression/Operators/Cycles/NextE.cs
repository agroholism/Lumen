using StandartLibrary.Expressions;
using StandartLibrary;
using System;
using System.Collections.Generic;

namespace Stereotype {
	public class NextE : System.Exception, Expression {
		public Value Eval(Scope e) {
			throw this;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			return this;
		}

		public Expression Optimize(Scope scope) {
			return this;
		}
	}
}
