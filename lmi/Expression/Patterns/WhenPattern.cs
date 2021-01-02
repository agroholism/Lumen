using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lang.Patterns {
	/// <summary> Pattern (subpattern) when (condition) </summary>
	internal class WhenPattern : IPattern {
		private IPattern subpattern;
		private Expression condition;

		public WhenPattern(IPattern subpattern, Expression condition) {
			this.subpattern = subpattern;
			this.condition = condition;
		}

		public MatchResult Match(Value value, Scope scope) {
			return new MatchResult(
				this.subpattern.Match(value, scope).IsSuccess && this.condition.Eval(scope).ToBoolean()
			);
		}

		public List<String> GetDeclaredVariables() {
			return this.subpattern.GetDeclaredVariables();
		}

		public IPattern Closure(ClosureManager manager) {
			return new WhenPattern(this.subpattern.Closure(manager), this.condition.Closure(manager));
		}
	}
}