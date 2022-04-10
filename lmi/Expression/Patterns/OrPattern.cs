using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;
using System;

namespace Lumen.Lang.Patterns {
    internal class OrPattern : IPattern {
        private IPattern result;
        private IPattern second;
		public Boolean IsNotEval => false;

		public OrPattern(IPattern result, IPattern second) {
            this.result = result;
            this.second = second;
        }

        public IPattern Closure(ClosureManager manager) {
            return new OrPattern(
                this.result.Closure(manager), 
                this.second.Closure(manager));
        }

        public List<String> GetDeclaredVariables() {
            List<String> res = new List<String>();
            res.AddRange(this.result.GetDeclaredVariables());
            res.AddRange(this.second.GetDeclaredVariables());
            return res;
        }

		public MatchResult Match(IValue value, Scope scope) {
			MatchResult result1 = this.result.Match(value, scope);

			if(!result1.IsSuccess) {
				result1 = this.second.Match(value, scope);
			}

			return result1;
        }

        public override String ToString() {
            return $"{this.result} | {this.second}";
        }
    }
}