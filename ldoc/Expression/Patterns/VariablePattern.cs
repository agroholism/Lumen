using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;
using System;

namespace ldoc {
    internal class VariablePattern : IPattern {
        private System.String id;
		public Boolean IsNotEval => false;
		public VariablePattern(System.String id) {
            this.id = id;
        }

        public Expression Closure(ClosureManager manager) {
            return new ValuePattern(manager.Scope[this.id]);
        }
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public Value Eval(Scope e) {
            throw new System.NotImplementedException();
        }

        public List<System.String> GetDeclaredVariables() {
            return new List<System.String>();
        }

        public MatchResult Match(Value value, Scope scope) {
            return new ValuePattern(scope[this.id]).Match(value, scope);
        }

        public override System.String ToString() {
            return this.id;
        }
    }
}