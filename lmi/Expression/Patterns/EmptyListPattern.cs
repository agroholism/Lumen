using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Lmi {
	/// <summary> Pattern [] </summary>
	internal class EmptyListPattern : IPattern {
		public static EmptyListPattern Instance { get; } = new EmptyListPattern();
		public Boolean IsNotEval { get; } = false;

		private EmptyListPattern() {

		}

        public MatchResult Match(Value value, Scope scope) {
            if (value is List list && LinkedList.IsEmpty(list.Value)) {
                return MatchResult.True;
            }

            return new MatchResult {
				Success = false,
				Note = "function wait an empty list"
			};
        }
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public List<String> GetDeclaredVariables() {
			return new List<String>();
		}

		public Value Eval(Scope e) {
			throw new NotImplementedException();
		}

		public Expression Closure(ClosureManager manager) {
			return this;
		}

		public override String ToString() {
            return "[]";
        }
    }
}