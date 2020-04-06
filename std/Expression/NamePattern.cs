using System;
using System.Collections.Generic;

namespace Lumen.Lang.Expressions {
    public class NamePattern : IPattern {
        private readonly String identifier;

		

		public NamePattern(String identifier) {
			this.identifier = identifier;
        }

		public Expression Closure(ClosureManager manager) {
			manager.Declare(this.identifier);

            return this;
        }

        public Value Eval(Scope e) {
            throw new NotImplementedException();
        }

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public List<String> GetDeclaredVariables() {
            return new List<String> { this.identifier };
        }

        public MatchResult Match(Value value, Scope scope) {
            scope[this.identifier] = value;
            return MatchResult.True;
        }

        public override String ToString() {
            return this.identifier.ToString();
        }
    }
}
