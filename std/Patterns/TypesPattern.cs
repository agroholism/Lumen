using System;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Lang.Patterns {
	internal class TypesPattern : IPattern {
		private IPattern subpattern;
		private List<IType> requirements;

		public TypesPattern(String subpattern, List<IType> requirements) {
			this.subpattern = new NamePattern(subpattern);
			this.requirements = requirements;
		}

		public TypesPattern(IPattern subpattern, List<IType> requirements) {
			this.subpattern = subpattern;
			this.requirements = requirements;
		}

		public MatchResult Match(Value value, Scope scope) {
			if (this.requirements.All(i => i.IsParentOf(value))) {
				return this.subpattern.Match(value, scope);
			}

			return new MatchResult(
				MatchResultKind.Fail,
				$"wait value of type {String.Join(", ", this.requirements)} given {value.Type}"
			);
		}

		public IPattern Closure(ClosureManager manager) {
			return new TypesPattern(this.subpattern.Closure(manager), this.requirements);
		}

		public List<String> GetDeclaredVariables() {
			return new List<String>();
		}

		public override String ToString() {
			return $"({this.subpattern}: {String.Join(", ", this.requirements)})";
		}
	}
}
