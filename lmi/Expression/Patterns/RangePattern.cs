using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class RangePattern : IPattern {
		private IPattern left;
		private IPattern right;
		private Boolean isInclusive;

		public RangePattern(IPattern left, IPattern right, Boolean isInclusive) {
			this.left = left;
			this.right = right;
			this.isInclusive = isInclusive;
		}

		public MatchResult Match(Value value, Scope scope) {
			Value leftValue = (this.left as ValuePattern)?.value;
			Value rightValue = (this.right as ValuePattern)?.value;

			if (leftValue == null && rightValue == null) {
				return MatchResult.True;
			}

			if (leftValue == null) {
				Fun _comparator = rightValue.Type.GetMember("compare", scope).ToFunction(scope);

				try {
					Double rightResult = _comparator.Run(scope, value, rightValue).ToDouble(scope);

					Boolean result = this.isInclusive ? rightResult <= 0 : rightResult < 0;

					if (result) {
						return MatchResult.True;
					}
				}
				catch {

				}

				return new MatchResult {
					Success = false,
					Note = $"expect value in range {leftValue}{(this.isInclusive ? "..." : "..")}{rightValue}"
				};
			}

			Fun comparator = leftValue.Type.GetMember("compare", scope).ToFunction(scope);

			if (rightValue == null) {
				try {
					Boolean result = comparator.Run(new Scope(scope), leftValue, value).ToDouble(scope) <= 0;

					if (result) {
						return MatchResult.True;
					}
				}
				catch {

				}

				return new MatchResult {
					Success = false,
					Note = $"expect value in range {leftValue}{(this.isInclusive ? "..." : "..")}{rightValue}"
				};
			}

			try {
				Double leftResult = comparator.Run(new Scope(scope), leftValue, value).ToDouble(scope);
				Double rightResult = comparator.Run(scope, value, rightValue).ToDouble(scope);

				Boolean result = leftResult <= 0 && (this.isInclusive ? rightResult <= 0 : rightResult < 0);

				if (result) {
					return MatchResult.True;
				}
			}
			catch {

			}

			return new MatchResult {
				Success = false,
				Note = $"expect value in range {leftValue}{(this.isInclusive ? "..." : "..")}{rightValue}"
			};
		}


		public IEnumerable<Value> EvalWithYield(Scope scope) {
			throw new NotImplementedException();
		}

		public List<String> GetDeclaredVariables() {
			return new List<String>();
		}

		public Value Eval(Scope e) {
			throw new NotImplementedException();
		}

		public Expression Closure(ClosureManager manager) {
			return new RangePattern(this.left?.Closure(manager) as IPattern, this.right?.Closure(manager) as IPattern, this.isInclusive);
		}

		public override String ToString() {
			return $"{this.left}{(this.isInclusive ? "..." : "..")}{this.right}";
		}
	}
}