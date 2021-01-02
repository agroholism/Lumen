using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lang.Patterns {
	internal class NotPattern : IPattern {
		private IPattern pattern;

		public NotPattern(IPattern pattern) {
			this.pattern = pattern;
		}

        public IPattern Closure(ClosureManager manager) {
            return new NotPattern(this.pattern.Closure(manager));
        }

        public List<String> GetDeclaredVariables() {
            return pattern.GetDeclaredVariables();
        }

        public MatchResult Match(Value value, Scope scope) {
            MatchResult result = this.pattern.Match(value, scope);

            if (result.IsSuccess) {
                return new MatchResult(MatchResultKind.Fail);
            }

            return MatchResult.Success;
        }

        public override String ToString() {
            return $"not {this.pattern}";
        }
    }
}