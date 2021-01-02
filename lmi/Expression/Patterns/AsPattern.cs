using System;
using System.Collections.Generic;

namespace Lumen.Lang.Patterns {
    internal class AsPattern : IPattern {
        private readonly IPattern innerPattern;
        private readonly String identifier;

		public AsPattern(IPattern innerPattern, String identifier) {
            this.innerPattern = innerPattern;
            this.identifier = identifier;
        }

        public IPattern Closure(ClosureManager manager) {
			manager.Declare(this.identifier);
            return new AsPattern(this.innerPattern.Closure(manager), this.identifier);
        }

        public List<String> GetDeclaredVariables() {
            List<String> result = new List<String> { this.identifier };
            result.AddRange(this.innerPattern.GetDeclaredVariables());
            return result;
        }

        public MatchResult Match(Value value, Scope scope) {
			MatchResult result = this.innerPattern.Match(value, scope);

			if (result.IsSuccess) {
                scope[this.identifier] = value;
                return MatchResult.Success;
            }

            return result;
        }

		public override String ToString() {
            return $"{this.innerPattern} as {this.identifier}";
        }
    }
}