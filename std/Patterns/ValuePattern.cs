using System;
using System.Collections.Generic;

namespace Lumen.Lang.Patterns {
    /// <summary> Pattern (value) </summary>
	public class ValuePattern : IPattern {
        public Value value;

		public ValuePattern(Value value) {
            this.value = value;
        }

		public MatchResult Match(Value value, Scope scope) {
			return new MatchResult(this.value.Equals(value));
        }

        public List<String> GetDeclaredVariables() {
            return new List<String>();
        }

        public IPattern Closure(ClosureManager manager) {
            return this;
        }

        public override String ToString() {
            return this.value.ToString();
        }
    }
}