using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Lmi {
	/// <summary> Pattern [||] </summary>
	internal class EmptyArrayPattern : IPattern {
		public static EmptyArrayPattern Instance { get; } = new EmptyArrayPattern();

		public Boolean IsNotEval { get; } = false;

		private EmptyArrayPattern() {

		}

		public MatchResult Match(Value value, Scope scope) {
			if (value is Lang.Array list && list.ToList(scope).Count == 0) {
				return MatchResult.True;
			}

			return new MatchResult {
				Success = false,
				Note = "function wait an empty array"
			};
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public List<String> GetDeclaredVariables() {
			return new List<String>();
		}

		public Value Eval(Scope e) {
			throw new NotImplementedException();
		}

		public Expression Closure(ClosureManager manager) {
			return this;
		}

		public override String ToString() {
			return "[||]";
		}
	}
}