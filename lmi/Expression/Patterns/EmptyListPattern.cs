using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Lang.Patterns {
	/// <summary> Pattern [] </summary>
	internal class EmptyListPattern : IPattern {
		public static EmptyListPattern Instance { get; } = new EmptyListPattern();

		private EmptyListPattern() {

		}

        public MatchResult Match(IValue value, Scope scope) {
            if (value is List list && LinkedList.IsEmpty(list.Value)) {
                return MatchResult.Success;
            }

            return new MatchResult (
				MatchResultKind.Fail,
				"function wait an empty list"
			);
        }

		public List<String> GetDeclaredVariables() {
			return new List<String>();
		}

		public IPattern Closure(ClosureManager manager) {
			return this;
		}

		public override String ToString() {
            return "[]";
        }
    }
}