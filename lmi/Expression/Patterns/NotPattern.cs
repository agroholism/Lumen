using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class NotPattern : IPattern {
		private IPattern pattern;

		public NotPattern(IPattern pattern) {
			this.pattern = pattern;
		}

        public Expression Closure(ClosureManager manager) {
            return new NotPattern(this.pattern.Closure(manager) as IPattern);
        }

        public Value Eval(Scope e) {
            throw new NotImplementedException();
        }

        public List<String> GetDeclaredVariables() {
            return new List<String>();
        }

        public MatchResult Match(Value value, Scope scope) {
            MatchResult result = this.pattern.Match(value, scope);

            if (result.Success) {
                return new MatchResult { Success = false };
            }

            return MatchResult.True;
        }

        public IEnumerable<Value> EvalWithYield(Scope scope) {
            throw new NotImplementedException();
        }

        public override String ToString() {
            return $"not {this.pattern}";
        }
    }
}