using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace ldoc {
    internal class WherePattern : IPattern {
        private IPattern result;
        private Expression exp;
		public Boolean IsNotEval => false;
		public WherePattern(IPattern result, Expression exp) {
            this.result = result;
            this.exp = exp;
        }
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public Expression Closure(ClosureManager manager) {
            return this;
        }

        public Value Eval(Scope e) {
            throw new System.NotImplementedException();
        }

        public List<System.String> GetDeclaredVariables() {
            return this.result.GetDeclaredVariables();
        }

        public MatchResult Match(Value value, Scope scope) {
            return new MatchResult { Success = this.result.Match(value, scope).Success && this.exp.Eval(scope).ToBoolean() };
        }
    }
}