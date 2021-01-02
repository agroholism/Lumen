using System;
using System.Collections.Generic;

namespace Lumen.Lang.Patterns {
	internal class ExactTypePattern : IPattern {
		private IPattern subpattern;
		private IType requirement;

		public ExactTypePattern(String subpattern, IType requirement) {
			this.subpattern = new NamePattern(subpattern);
			this.requirement = requirement;
		}

		public ExactTypePattern(IPattern subpattern, IType requirement) {
			this.subpattern = subpattern;
			this.requirement = requirement;
		}

		public MatchResult Match(Value value, Scope scope) {
			if (this.requirement.IsParentOf(value)) {
				return this.subpattern.Match(value, scope);
			}

			return new MatchResult(
				MatchResultKind.Fail,
				$"wait value of type {this.requirement} given {value.Type}"
			);
		}

		public IPattern Closure(ClosureManager manager) {
			return new ExactTypePattern(this.subpattern.Closure(manager), this.requirement);
		}

		public List<String> GetDeclaredVariables() {
			return new List<String>();
		}

		public override String ToString() {
			return $"({this.subpattern}: {this.requirement})";
		}
	}
}
