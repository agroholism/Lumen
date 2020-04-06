using System;
using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace ldoc {
    internal class DiscordPattern : IPattern {
        public Expression Closure(ClosureManager manager) {
            return this;
        }
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public Boolean IsNotEval => false;
		public Value Eval(Scope e) {
            throw new NotImplementedException();
        }

        public List<String> GetDeclaredVariables() {
            return new List<String>();
        }

        public MatchResult Match(Value value, Scope scope) {
            return MatchResult.True;
        }

        public override String ToString() {
            return "_";
        }
    }
}