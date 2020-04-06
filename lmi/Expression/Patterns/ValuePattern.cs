using System;
using System.Collections.Generic;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class ValuePattern : IPattern {
        private Value value;

		public ValuePattern(Value value) {
            this.value = value;
        }

        public Expression Closure(ClosureManager manager) {
            return this;
        }

        public Value Eval(Scope e) {
            throw new NotImplementedException();
        }

        public List<String> GetDeclaredVariables() {
            return new List<String>();
        }

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public MatchResult Match(Value value, Scope scope) {
			return new MatchResult { Success = this.value.Equals(value) };
        }

        public override String ToString() {
            return this.value.ToString();
        }
    }
}