using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;
using System;

namespace Lumen.Lmi {
    internal class OrPattern : IPattern {
        private IPattern result;
        private IPattern second;
		public Boolean IsNotEval => false;

		public OrPattern(IPattern result, IPattern second) {
            this.result = result;
            this.second = second;
        }

        public Expression Closure(ClosureManager manager) {
            return new OrPattern(
                this.result.Closure(manager) as IPattern, 
                this.second.Closure(manager) as IPattern);
        }

        public Value Eval(Scope e) {
            throw new System.NotImplementedException();
        }

        public List<System.String> GetDeclaredVariables() {
            List<System.String> res = new List<System.String>();
            res.AddRange(this.result.GetDeclaredVariables());
            res.AddRange(this.second.GetDeclaredVariables());
            return res;
        }


		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public MatchResult Match(Value value, Scope scope) {
			var result1 = this.result.Match(value, scope);

			if(!result1.Success) {
				result1 = this.second.Match(value, scope);
			}

			return result1;
        }

        public override System.String ToString() {
            return $"{this.result} | {this.second}";
        }
    }
}