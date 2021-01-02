using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lang.Patterns {
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
				return MatchResult.Success;
			}

			if (leftValue == null) {
				Fun _comparator = rightValue.Type.GetMember("compare", scope).ToFunction(scope);

				try {
					Double rightResult = _comparator.Call(scope, value, rightValue).ToDouble(scope);

					Boolean result = this.isInclusive ? rightResult <= 0 : rightResult < 0;

					if (result) {
						return MatchResult.Success;
					}
				}
				catch {

				}

				return new MatchResult (
					MatchResultKind.Fail,
					$"expect value in range {leftValue}{(this.isInclusive ? "..." : "..")}{rightValue}"
				);
			}

			Fun comparator = leftValue.Type.GetMember("compare", scope).ToFunction(scope);

			if (rightValue == null) {
				try {
					Boolean result = comparator.Call(new Scope(scope), leftValue, value).ToDouble(scope) <= 0;

					if (result) {
						return MatchResult.Success;
					}
				}
				catch {

				}

				return new MatchResult(
					MatchResultKind.Fail,
					$"expect value in range {leftValue}{(this.isInclusive ? "..." : "..")}{rightValue}"
				);
			}

			try {
				Double leftResult = comparator.Call(new Scope(scope), leftValue, value).ToDouble(scope);
				Double rightResult = comparator.Call(scope, value, rightValue).ToDouble(scope);

				Boolean result = leftResult <= 0 && (this.isInclusive ? rightResult <= 0 : rightResult < 0);

				if (result) {
					return MatchResult.Success;
				}
			}
			catch {

			}

			return new MatchResult (
				MatchResultKind.Fail,
				$"expect value in range {leftValue}{(this.isInclusive ? "..." : "..")}{rightValue}"
			);
		}


		public List<String> GetDeclaredVariables() {
			return new List<String>();
		}

		public IPattern Closure(ClosureManager manager) {
			return new RangePattern(this.left?.Closure(manager), this.right?.Closure(manager), this.isInclusive);
		}

		public override String ToString() {
			return $"{this.left}{(this.isInclusive ? "..." : "..")}{this.right}";
		}
	}
}