using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;
using System;

namespace ldoc {
    internal class UnitPattern : IPattern {
        public Expression Closure(ClosureManager manager) {
            return this;
        }
		public Boolean IsNotEval => false;
		public Value Eval(Scope e) {
            throw new System.NotImplementedException();
        }
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public List<System.String> GetDeclaredVariables() {
            return new List<System.String>();
        }

        public MatchResult Match(Value value, Scope scope) {
            return new MatchResult { Success = value == Const.UNIT };
        }

        public override System.String ToString() {
            return "()";
        }
    }
}