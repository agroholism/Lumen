using System;
using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace ldoc {
    internal class EmptyListPattern : IPattern {
        public Expression Closure(ClosureManager manager) {
            return this;
        }
		public Boolean IsNotEval => false;
		public Value Eval(Scope e) {
            throw new NotImplementedException();
        }
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public List<String> GetDeclaredVariables() {
            return new List<String>();
        }

        public MatchResult Match(Value value, Scope scope) {
            if (value is List list && LinkedList.IsEmpty(list.Value)) {
                return MatchResult.True;
            }

			if (value is LazyList lll) {
				if(lll.Next == null)
					return MatchResult.True;
			}

			if (value is Lazy lazy && lazy.Force() is List ll && LinkedList.IsEmpty(ll.Value)) {
				return MatchResult.True;
			}

            return new MatchResult {
				Success = false,
				Note = "unction wait an empty List"
			};
        }

        public override String ToString() {
            return "[]";
        }
    }
}