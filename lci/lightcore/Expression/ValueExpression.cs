using System;
using System.Collections.Generic;

using StandartLibrary;
using StandartLibrary.Expressions;

namespace Stereotype {
	[Serializable]
	public class ValueE : Expression {
		public Value val;

		public ValueE(Value Object) {
			this.val = Object;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			return this;
		}
		public Expression Optimize(Scope scope) {
			return this;
		}

		public ValueE(BigFloat Object) {
			this.val = new Num(Object);
		}

		public ValueE(String Object) {
			this.val = new KString(Object);
		}

		public Value Eval(Scope e) {
			return val;
		}

		public override String ToString() {
			return this.val.ToString();
		}
	}
}
