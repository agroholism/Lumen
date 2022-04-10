using System.Collections.Generic;
using System;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Lang.Patterns {
	/// <summary> Pattern () </summary>
	internal class UnitPattern : IPattern {
		public static UnitPattern Instance = new UnitPattern();

		private UnitPattern() {

		}

		public MatchResult Match(IValue value, Scope scope) {
			return new MatchResult(value == Const.UNIT);
		}

		public List<String> GetDeclaredVariables() {
			return new List<String>();
		}

		public IPattern Closure(ClosureManager manager) {
			return this;
		}

		public override String ToString() {
			return "()";
		}
	}
}