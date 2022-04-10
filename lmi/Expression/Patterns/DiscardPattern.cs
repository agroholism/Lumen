using System;
using System.Collections.Generic;
using Lumen.Lang.Patterns;
using Lumen.Lang;

namespace Lumen.Lang.Patterns {
    internal class DiscardPattern : IPattern {
		public static DiscardPattern Instance { get; } = new DiscardPattern();

		private DiscardPattern() {

		}

        public MatchResult Match(IValue value, Scope scope) {
            return MatchResult.Success;
        }

        public IPattern Closure(ClosureManager manager) {
            return this;
        }

        public List<String> GetDeclaredVariables() {
            return new List<String>();
        }

        public override String ToString() {
            return "_";
        }
    }
}