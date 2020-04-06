using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;
using System;

namespace ldoc {
	internal class ValuePattern : IPattern {
        private Value value;
		public Boolean IsNotEval => false;
		public ValuePattern(Value value) {
            this.value = value;
        }
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
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

        public MatchResult Match(Value value, Scope scope) {
			if(this.value is Lazy lazy) {
				this.value = lazy.Force();
			}

			if (value is Lazy lazy2) {
				value = lazy2.Force();
			}

			return new MatchResult { Success = this.value.Equals(value) };
        }

        public override String ToString() {
            return this.value.ToString();
        }
    }
}