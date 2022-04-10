using System;
using System.Collections.Generic;

namespace Lumen.Lang.Patterns {
    /// <summary> Pattern (value) </summary>
	public class ValuePattern : IPattern {
        public IValue value;

		public ValuePattern(IValue value) {
            this.value = value;
        }

		public MatchResult Match(IValue value, Scope scope) {
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