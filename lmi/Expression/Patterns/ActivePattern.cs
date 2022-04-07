using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lang.Patterns {
	internal class ActivePattern : IPattern {
		private Expression patternFunction;
		private List<IPattern> subpatterns;

		public ActivePattern(Expression idExpression, List<IPattern> subpatterns) {
			this.patternFunction = idExpression;
			this.subpatterns = subpatterns;
		}

		public MatchResult Match(Value value, Scope scope) {
			Fun testFunction = this.patternFunction.Eval(scope).ToFunction(scope);

			Value testResult = testFunction.Call(new Scope(scope), value);
			if (Prelude.Some.IsParentOf(testResult)) {
				Value val = Prelude.DeconstructSome(testResult);

				List<Value> results = val.ToFlow(scope).ToList();
				Int32 index = 0;
				foreach (IPattern subpattern in this.subpatterns) {
					MatchResult matchResult = subpattern.Match(results[index], scope);

					if (!matchResult.IsSuccess) {
						return matchResult;
					}

					index++;
				}

				return MatchResult.Success;
			}

			return new MatchResult(
				MatchResultKind.Fail
			);
		}

		public IPattern Closure(ClosureManager manager) {
			return new ActivePattern(this.patternFunction.Closure(manager), this.subpatterns.Select(x => x.Closure(manager)).ToList());
		}

		public List<String> GetDeclaredVariables() {
			List<String> result = new List<String>();

			foreach (IPattern pattern in this.subpatterns) {
				result.AddRange(pattern.GetDeclaredVariables());
			}

			return result;
		}
	}
}