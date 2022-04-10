using System;
using System.Collections.Generic;

namespace Lumen.Lang.Patterns {
	public class ValuePattern : IPattern {
        public IValue Value { get; private set; }

		public ValuePattern(IValue value) {
            this.Value = value;
        }

		public MatchResult Match(IValue value, Scope scope) {
			return new MatchResult(this.Value.Equals(value));
        }

        public List<String> GetDeclaredVariables() {
            return new List<String>();
        }

        public IPattern Closure(ClosureManager manager) {
            return this;
        }

        public override String ToString() {
            return this.Value.ToString();
        }
    }
}