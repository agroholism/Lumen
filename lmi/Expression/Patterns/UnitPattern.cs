using System.Collections.Generic;
using System;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Lmi {
	/// <summary> Pattern () </summary>
	internal class UnitPattern : IPattern {
		public static UnitPattern Instance = new UnitPattern();

		public Boolean IsNotEval => false;

		private UnitPattern() {

		}

		public MatchResult Match(Value value, Scope scope) {
			return new MatchResult(value == Const.UNIT);
		}

		public List<String> GetDeclaredVariables() {
			return new List<String>();
		}

		public Value Eval(Scope e) {
			throw new NotImplementedException();
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public Expression Closure(ClosureManager manager) {
			return this;
		}

		public override String ToString() {
			return "()";
		}
	}
}