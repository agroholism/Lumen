using System;
using System.Collections.Generic;
using System.Linq;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class ActivePattern : IPattern {
		private Expression patternFunction;
		private List<IPattern> subpatterns;
		private List<Expression> arguments;

		public ActivePattern(Expression idExpression, List<Expression> arguments, List<IPattern> subpatterns) {
			this.patternFunction = idExpression;
			this.arguments = arguments;
			this.subpatterns = subpatterns;
		}

		public Expression Closure(ClosureManager manager) {
			return new ActivePattern(this.patternFunction.Closure(manager), this.arguments.Select(x => x.Closure(manager)).ToList(), this.subpatterns.Select(x => x.Closure(manager) as IPattern).ToList());
		}

		public Value Eval(Scope e) {
			throw new NotImplementedException();
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			throw new NotImplementedException();
		}

		public List<String> GetDeclaredVariables() {
			List<String> result = new List<String>();

			foreach (IPattern pattern in this.subpatterns) {
				result.AddRange(pattern.GetDeclaredVariables());
			}

			return result;
		}

		public MatchResult Match(Value value, Scope scope) {
			Fun f = this.patternFunction.Eval(scope) as Fun;
			Value result;
			if (this.arguments.Count > 0) {
				List<Value> l = this.arguments.Select(i => i.Eval(scope)).ToList();
				result = f.Run(new Scope(scope), new Lang.Array(l), value);
			}
			else {
				result = f.Run(new Scope(scope), value);
			}

			if (Prelude.Some.IsParentOf(result)) {
				Value inner = Prelude.DeconstructSome(result);

				List<Value> results = inner.ToStream(scope).ToList();
				Int32 index = 0;
				foreach (IPattern subpattern in this.subpatterns) {
					MatchResult matchResult = subpattern.Match(results[index], scope);
					if (!matchResult.Success) {
						return matchResult;
					}
					index++;
				}
				return MatchResult.True;
			}

			return new MatchResult {
				Success = false
			};
		}
	}
}