using System;
using System.Collections.Generic;

namespace Lumen.Lang.Patterns {
	public class NamePattern : IPattern {
		private readonly String identifier;

		public NamePattern(String identifier) {
			this.identifier = identifier;
		}

		public MatchResult Match(Value value, Scope scope) {
			/*if(scope.ExistsInThisScope(this.identifier)) {
				Console.WriteLine($"WARNING: there are rebinding {this.identifier} please don't use rebindings");
			}*/

			scope.Bind(this.identifier, value);
			return MatchResult.Success;
		}

		public IPattern Closure(ClosureManager manager) {
			manager.Declare(this.identifier);
			return this;
		}

		public List<String> GetDeclaredVariables() {
			return new List<String> { this.identifier };
		}


		public override String ToString() {
			return this.identifier.ToString();
		}
	}
}
