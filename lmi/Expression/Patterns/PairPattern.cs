using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class PairPattern : IPattern {
		private IPattern result;
		private IPattern subp;

		public Boolean IsNotEval => false;

		public PairPattern(IPattern result, IPattern subp) {
			this.result = result;
			this.subp = subp;
		}

		public Expression Closure(ClosureManager manager) {
			return new PairPattern(
				this.result.Closure(manager) as IPattern,
				this.subp.Closure(manager) as IPattern);
		}

		public Value Eval(Scope e) {
			throw new NotImplementedException();
		}

		public List<String> GetDeclaredVariables() {
			List<String> result = new List<String>();

			result.AddRange(this.result.GetDeclaredVariables());
			result.AddRange(this.result.GetDeclaredVariables());

			return result;
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public MatchResult Match(Value value, Scope scope) {
			if (value is Instance ins && ins.Type is Constructor ctor && ctor.Parent == Prelude.Pair) {
				return new MatchResult {
					Success = this.result.Match(ins.items[0], scope).Success && this.subp.Match(ins.items[1], scope).Success
				};
			}

			return new MatchResult { Success = false };
		}
	}
}