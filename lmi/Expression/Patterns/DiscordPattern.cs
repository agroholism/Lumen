using System;
using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Lmi {
    internal class DiscordPattern : IPattern {
		public static DiscordPattern Instance { get; } = new DiscordPattern();
		public Boolean IsNotEval { get; } = true;
		private DiscordPattern() {

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
            return MatchResult.True;
        }

        public override String ToString() {
            return "_";
        }
    }
}