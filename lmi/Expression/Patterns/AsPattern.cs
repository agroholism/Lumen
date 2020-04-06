using System;
using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Lmi {
    internal class AsPattern : IPattern {
        private readonly IPattern innerPattern;
        private readonly String identifier;

		public AsPattern(IPattern innerPattern, String identifier) {
            this.innerPattern = innerPattern;
            this.identifier = identifier;
        }

        public Expression Closure(ClosureManager manager) {
			manager.Declare(this.identifier);
            return new AsPattern(this.innerPattern.Closure(manager) as IPattern, this.identifier);
        }

        public Value Eval(Scope e) {
            throw new NotImplementedException();
        }

        public List<String> GetDeclaredVariables() {
            List<String> result = new List<String> { this.identifier };
            result.AddRange(this.innerPattern.GetDeclaredVariables());
            return result;
        }

        public MatchResult Match(Value value, Scope scope) {
			MatchResult result = this.innerPattern.Match(value, scope);

			if (result.Success) {
                scope[this.identifier] = value;
                return MatchResult.True;
            }

            return result;
        }

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public override String ToString() {
            return $"{this.innerPattern.ToString()} as {this.identifier}";
        }
    }
}