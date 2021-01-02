using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Lang.Patterns {
	/// <summary> Pattern @[] </summary>
	internal class EmptyArrayPattern : IPattern {
		public static EmptyArrayPattern Instance { get; } = new EmptyArrayPattern();

		private EmptyArrayPattern() {

		}

		public MatchResult Match(Value value, Scope scope) {
			if (value is MutArray list && list.ToList(scope).Count == 0) {
				return MatchResult.Success;
			}

			return new MatchResult(
				MatchResultKind.Fail,
				"function wait an empty array"
			);
		}

		public List<String> GetDeclaredVariables() {
			return new List<String>();
		}

		public IPattern Closure(ClosureManager manager) {
			return this;
		}

		public override String ToString() {
			return "@[]";
		}
	}
}